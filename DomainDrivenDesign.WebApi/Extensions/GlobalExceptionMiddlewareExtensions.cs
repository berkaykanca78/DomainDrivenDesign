using DomainDrivenDesign.WebApi.Middlewares;

namespace DomainDrivenDesign.WebApi.Extensions
{
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
} 