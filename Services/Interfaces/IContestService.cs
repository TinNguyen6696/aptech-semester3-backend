using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Contests;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IContestService
    {
        Task<Result<ContestListDto>> GetContestsAsync(TalentCategory? category, int page, int pageSize);
        Task<Result<ContestDto>> GetContestByIdAsync(int id);
        Task<Result<ContestDto>> CreateContestAsync(int userId, CreateContestRequest request);
        Task<Result<ContestDto>> UpdateContestAsync(int contestId, UpdateContestRequest request);
        Task<Result<object>> DeleteContestAsync(int contestId);

        Task<Result<ContestEntryListDto>> GetEntriesAsync(int contestId, int page, int pageSize, int? currentUserId);
        Task<Result<ContestEntryDto>> AddEntryAsync(int userId, int contestId, CreateContestEntryRequest request);
        Task<Result<object>> WithdrawEntryAsync(int userId, int contestId, int entryId, bool isAdmin);
        Task<Result<object>> ToggleVoteAsync(int userId, int contestId, int entryId);
    }
}