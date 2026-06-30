using System.Text.Json;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Middlewares
{
    public class JtiDenylistMiddleware
    {
        private readonly RequestDelegate _next;

        public JtiDenylistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtDenylistRepository jwtDenylistRepo)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var jti = context.User.FindFirst("jti")?.Value;

                if (jti != null && await jwtDenylistRepo.ExistsAsync(jti))
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    var result = new Result<object>
                    {
                        IsSuccess = false,
                        Message = "Token has been revoked.",
                        StatusCode = 401
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                    return;
                }
            }

            await _next(context);
        }
    }
}
