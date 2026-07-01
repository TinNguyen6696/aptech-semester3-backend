using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class ProvincesController : BaseApiController
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _provinceService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }
    }
}
