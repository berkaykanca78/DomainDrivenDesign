using DomainDrivenDesign.Infrastructure.Helpers;
using DomainDrivenDesign.Infrastructure.Settings;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using DomainDrivenDesign.WebApi.Extensions;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

#region Config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<LoginSettings>(builder.Configuration.GetSection("LoginSettings"));

// Memory Cache servisini ekle
builder.Services.AddMemoryCache();

// Rate limiting yapılandırması
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.RealIpHeader = "X-Forwarded-For";
    options.ClientIdHeader = "X-ClientId";
    options.HttpStatusCode = 429;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 1
        }
    };
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
#endregion

#region Controller, CORS, Swagger
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
#endregion

#region Authentication and JwtSettings
builder.Services.AddSingleton<JwtSettings>(resolver =>
{
    var encryptedJwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    
    var jwtSettings = new JwtSettings
    {
        Issuer = EncryptionHelper.Decrypt(encryptedJwtSettings.Issuer),
        Audience = EncryptionHelper.Decrypt(encryptedJwtSettings.Audience),
        SecretKey = EncryptionHelper.Decrypt(encryptedJwtSettings.SecretKey)
    };
    
    return jwtSettings;
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)) 
        };
    });
#endregion

var app = builder.Build();

#region Middleware Pipelines
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/scalar/");
        return;
    }
    await next();
});

app.UseGlobalExceptionMiddleware(); // Önce global exception handling
app.UseIpRateLimiting(); // Sonra rate limiting

app.UseHttpsRedirection();
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference(opt =>
{
    opt.WithTitle("Berkay Kanca - Domain Drive Design Projesi")
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();
app.Run();
#endregion
