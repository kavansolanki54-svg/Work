using DallyWorkReoprt.Models;
using System.Text.Json;

namespace DallyWorkReoprt.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted) return;

                _logger.LogError(ex, "Unhandled Exception");

                try
                {
                    using var scope = context.RequestServices.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DallyWorkReoprt.DAL.Models.ApplicationDbContext>();
                    var errorLog = new DallyWorkReoprt.DAL.Models.ErrorLog
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        Controller = context.Request.Path.Value?.ToString(),
                        Action = context.Request.Method,
                        CreatedOn = DateTime.Now
                    };
                    dbContext.ErrorLogs.Add(errorLog);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Failed to log error to database");
                }

                context.Response.StatusCode = StatusCodes.Status200OK; // Always 200 for client
                context.Response.ContentType = "application/json";

                var _e = new Dictionary<string, string[]>();

                var errorMessage = "Something went wrong. Please try again.";
                if (_env.IsDevelopment())
                    _e.Add("exception", [ex.Message, ex.StackTrace!]);
                else
                    _e.Add("exception", [ex.Message, ex.StackTrace!]);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var errorResponse = ApiResponse<Type?>.ErrorResponse(errorMessage, _e);
                var json = JsonSerializer.Serialize(errorResponse, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}

