using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TalentShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        protected int? CurrentUserIdOrNull
        {
            get
            {
                var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? null : int.Parse(value);
            }
        }

        protected string CurrentUserRole =>
            User.FindFirstValue(ClaimTypes.Role)!;

        protected string CurrentUserJti =>
            User.FindFirstValue("jti")!;

        protected DateTime CurrentUserTokenExpiry =>
            DateTimeOffset.FromUnixTimeSeconds(long.Parse(User.FindFirstValue("exp")!)).UtcDateTime;
    }
}
