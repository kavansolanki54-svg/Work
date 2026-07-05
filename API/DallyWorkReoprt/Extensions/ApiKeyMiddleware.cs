using DallyWorkReoprt.Models;
using System.Text.Json;

namespace DallyWorkReoprt.Extensions
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private const string APIKEYNAME = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Optional: Skip check for static files or Swagger if desired
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                await ReturnUnauthorized(context, "API Key was not provided.");
                return;
            }

            var apiKey = _configuration.GetValue<string>("MY_API_KEY");

            if (apiKey == null || !apiKey.Equals(extractedApiKey))
            {
                await ReturnUnauthorized(context, "Unauthorized client.");
                return;
            }

            await _next(context);
        }

        private static async Task ReturnUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var errorResponse = ApiResponse<Type?>.ErrorResponse(message, new Dictionary<string, string[]>
            {
                { "ApiKey", new[] { message } }
            });

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
