using System.Net;

namespace GameServer.Middleware
{
    public class ApiKeyAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        private readonly string apiKey = configuration["ADMIN_API_KEY"] ?? throw new InvalidOperationException("ADMIN_API_KEY environment variable is not configured");

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/admin"))
            {
                await next(context);
                return;
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            if (!context.Request.Headers.TryGetValue("X-Admin-Key", out var providedApiKey))
            {
                logger.LogWarning("Admin API access denied - missing X-Admin-Key header from IP {ClientIp}", clientIp);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing API key");
                return;
            }

            if (providedApiKey != apiKey)
            {
                logger.LogWarning("Admin API access denied - invalid API key from IP {ClientIp}", clientIp);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid API key");
                return;
            }

            logger.LogInformation("Admin API access granted from IP {ClientIp}", clientIp);
            await next(context);
        }
    }
}
