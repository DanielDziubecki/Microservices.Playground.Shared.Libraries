using Logging.Core.Handlers.WebApi;
using Microsoft.AspNetCore.Builder;

namespace Logging.Core.Extensions
{
    public static class LoggingExtensions
    {
        public static IApplicationBuilder UseHttpLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpLoggingMiddleware>();
        }
    }
}