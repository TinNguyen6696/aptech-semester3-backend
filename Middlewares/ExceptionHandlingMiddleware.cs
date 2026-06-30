using System.Text.Json;
using TalentShowcase.Api.Common;

namespace TalentShowcase.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorAsync(context, ex.Message);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var result = new Result<object>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred.",
                StatusCode = 500
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}
