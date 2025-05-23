using System.Net;
using System.Text.Json;
using Serilog;

namespace DomainDrivenDesign.WebApi.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                Log.Information("İstek başladı - {Path} - {Method} - {IP}", 
                    context.Request.Path, 
                    context.Request.Method,
                    context.Connection.RemoteIpAddress);

                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                if (context.Response.StatusCode == 429 || responseBody.Contains("API calls quota exceeded"))
                {
                    context.Response.Body = originalBodyStream;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "İstek limitiniz dolmuştur, lütfen bekleyiniz.",
                        DetailedMessage = responseBody,
                        Timestamp = DateTime.UtcNow
                    };

                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    var json = JsonSerializer.Serialize(response, options);

                    Log.Warning("Rate limiting tetiklendi. IP: {IP}, Path: {Path}, Response: {Response}",
                        context.Connection.RemoteIpAddress,
                        context.Request.Path,
                        json);

                    await context.Response.WriteAsync(json);
                    return;
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);

                Log.Information("İstek tamamlandı - {Path} - {Method} - {StatusCode}", 
                    context.Request.Path, 
                    context.Request.Method,
                    context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBodyStream;
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                DetailedMessage = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            var errorOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var errorJson = JsonSerializer.Serialize(errorResponse, errorOptions);

            Log.Error(exception, "Beklenmeyen bir hata oluştu. IP: {IP}, Path: {Path}, Response: {Response}",
                context.Connection.RemoteIpAddress,
                context.Request.Path,
                errorJson);

            await context.Response.WriteAsync(errorJson);
        }
    }
} 