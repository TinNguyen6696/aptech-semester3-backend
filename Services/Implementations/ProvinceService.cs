using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class ProvinceService : IProvinceService
    {
        private readonly IGenericRepository<Province> _provinceRepo;

        public ProvinceService(IGenericRepository<Province> provinceRepo)
        {
            _provinceRepo = provinceRepo;
        }

        public async Task<Result<IEnumerable<ProvinceDto>>> GetAllAsync()
        {
            var provinces = await _provinceRepo.GetAllAsync();

            var dtos = provinces
                .OrderBy(p => p.Id)
                .Select(p => new ProvinceDto { Id = p.Id, Name = p.Name });

            return new Result<IEnumerable<ProvinceDto>>
            {
                Data = dtos,
                IsSuccess = true,
                Message = "Provinces retrieved successfully.",
                StatusCode = 200
            };
        }
    }
}
