using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Controllers
{
    public class OptionsController : BaseApiController
    {
        [HttpGet]
        public IActionResult GetOptions()
        {
            var options = new OptionsDto
            {
                Roles = Enum.GetNames<UserRole>().Where(r => r != nameof(UserRole.Admin)),
                TalentCategories = Enum.GetNames<TalentCategory>(),
                SkillLevels = Enum.GetNames<SkillLevel>()
            };

            return Ok(new Result<OptionsDto>
            {
                Data = options,
                IsSuccess = true,
                Message = "Options retrieved successfully.",
                StatusCode = 200
            });
        }
    }
}
