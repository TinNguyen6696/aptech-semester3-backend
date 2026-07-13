using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByReferenceAsync(string referenceType, int referenceId, int page, int pageSize);
        Task<int> CountByReferenceAsync(string referenceType, int referenceId);
        Task<Dictionary<int, int>> CountByReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds);
        Task<Comment?> GetByIdWithUserAsync(int id);
        Task DeleteByReferenceAsync(string referenceType, int referenceId);
    }
}