using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IProvinceService
    {
        Task<Result<IEnumerable<ProvinceDto>>> GetAllAsync();
    }
}
