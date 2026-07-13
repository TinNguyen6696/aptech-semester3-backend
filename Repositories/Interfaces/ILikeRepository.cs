using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface ILikeRepository : IGenericRepository<Like>
    {
        Task<Like?> GetAsync(string referenceType, int referenceId, int userId);
        Task<int> CountByReferenceAsync(string referenceType, int referenceId);
        Task<Dictionary<int, int>> CountByReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds);
        Task DeleteByReferenceAsync(string referenceType, int referenceId);
    }
}
