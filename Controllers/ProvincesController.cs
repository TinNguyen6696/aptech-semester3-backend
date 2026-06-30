using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TalentShowcase.Api.Controllers
{
    public class ProvincesController : BaseApiController
    {
        private readonly AppDbContext _context;

        public ProvincesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Provinces
                .OrderBy(p => p.Id)
                .Select(p => new ProvinceDto { Id = p.Id, Name = p.Name })
                .ToListAsync();

            return Ok(new Result<IEnumerable<ProvinceDto>>
            {
                Data = provinces,
                IsSuccess = true,
                Message = "Provinces retrieved successfully.",
                StatusCode = 200
            });
        }
    }
}
