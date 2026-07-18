using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Contests;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class ContestService : IContestService
    {
        private const int MaxPageSize = 10;

        private readonly IContestRepository _contestRepo;
        private readonly IContestEntryRepository _entryRepo;
        private readonly IContestVoteRepository _voteRepo;
        private readonly IVideoRepository _videoRepo;

        public ContestService(
            IContestRepository contestRepo,
            IContestEntryRepository entryRepo,
            IContestVoteRepository voteRepo,
            IVideoRepository videoRepo)
        {
            _contestRepo = contestRepo;
            _entryRepo = entryRepo;
            _voteRepo = voteRepo;
            _videoRepo = videoRepo;
        }

        public async Task<Result<ContestListDto>> GetContestsAsync(TalentCategory? category, int page, int pageSize)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<ContestListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (page < 1)
                return new Result<ContestListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<ContestListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _contestRepo.CountPublicAsync(category);
            var contests = (await _contestRepo.GetPublicAsync(category, page, pageSize)).ToList();
            var entryCounts = await _entryRepo.CountByContestIdsAsync(contests.Select(c => c.Id));

            var result = new ContestListDto
            {
                Contests = contests.Select(c => ToDto(c, entryCounts.GetValueOrDefault(c.Id))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<ContestListDto> { Data = result, IsSuccess = true, Message = "Contests retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ContestDto>> GetContestByIdAsync(int id)
        {
            var contest = await _contestRepo.GetByIdAsync(id);
            if (contest == null)
                return new Result<ContestDto> { IsSuccess = false, Message = "Contest not found.", StatusCode = 404 };

            var entryCount = await _entryRepo.CountByContestIdAsync(id);
            return new Result<ContestDto> { Data = ToDto(contest, entryCount), IsSuccess = true, Message = "Contest retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ContestDto>> CreateContestAsync(int userId, CreateContestRequest request)
        {
            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<ContestDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (request.EndDate!.Value <= request.StartDate!.Value)
                return new Result<ContestDto> { IsSuccess = false, Message = "End date must be after start date.", StatusCode = 400 };

            var contest = new Contest
            {
                CreatedByUserId = userId,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category!.Value,
                StartDate = request.StartDate!.Value,
                EndDate = request.EndDate!.Value
            };

            await _contestRepo.AddAsync(contest);
            await _contestRepo.SaveChangesAsync();

            return new Result<ContestDto> { Data = ToDto(contest, 0), IsSuccess = true, Message = "Contest created successfully.", StatusCode = 201 };
        }

        public async Task<Result<ContestDto>> UpdateContestAsync(int contestId, UpdateContestRequest request)
        {
            var contest = await _contestRepo.GetByIdAsync(contestId);
            if (contest == null)
                return new Result<ContestDto> { IsSuccess = false, Message = "Contest not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<ContestDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (request.EndDate!.Value <= request.StartDate!.Value)
                return new Result<ContestDto> { IsSuccess = false, Message = "End date must be after start date.", StatusCode = 400 };

            contest.Title = request.Title;
            contest.Description = request.Description;
            contest.Category = request.Category!.Value;
            contest.StartDate = request.StartDate!.Value;
            contest.EndDate = request.EndDate!.Value;

            _contestRepo.Update(contest);
            await _contestRepo.SaveChangesAsync();

            var entryCount = await _entryRepo.CountByContestIdAsync(contestId);
            return new Result<ContestDto> { Data = ToDto(contest, entryCount), IsSuccess = true, Message = "Contest updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteContestAsync(int contestId)
        {
            var contest = await _contestRepo.GetByIdAsync(contestId);
            if (contest == null)
                return new Result<object> { IsSuccess = false, Message = "Contest not found.", StatusCode = 404 };

            _contestRepo.Remove(contest);
            await _contestRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Contest deleted successfully.", StatusCode = 200 };
        }

        public async Task<Result<ContestEntryListDto>> GetEntriesAsync(int contestId, int page, int pageSize, int? currentUserId)
        {
            var contest = await _contestRepo.GetByIdAsync(contestId);
            if (contest == null)
                return new Result<ContestEntryListDto> { IsSuccess = false, Message = "Contest not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<ContestEntryListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<ContestEntryListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _entryRepo.CountByContestIdAsync(contestId);
            var entries = (await _entryRepo.GetByContestIdAsync(contestId, page, pageSize)).ToList();

            var entryIds = entries.Select(e => e.Id).ToList();
            var voteCounts = await _voteRepo.CountByEntryIdsAsync(entryIds);
            var votedIds = currentUserId.HasValue
                ? await _voteRepo.GetVotedEntryIdsAsync(entryIds, currentUserId.Value)
                : null;

            // Entries already arrive ranked by vote count from the repository (DB-level, pre-pagination).
            var result = new ContestEntryListDto
            {
                Entries = entries.Select(e => ToDto(
                    e,
                    voteCounts.GetValueOrDefault(e.Id),
                    votedIds == null ? null : votedIds.Contains(e.Id))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<ContestEntryListDto> { Data = result, IsSuccess = true, Message = "Entries retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<MyContestEntryListDto>> GetMyEntriesAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<MyContestEntryListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<MyContestEntryListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _entryRepo.CountByEntrantUserIdAsync(userId);
            var entries = (await _entryRepo.GetByEntrantUserIdAsync(userId, page, pageSize)).ToList();
            var voteCounts = await _voteRepo.CountByEntryIdsAsync(entries.Select(e => e.Id).ToList());

            var result = new MyContestEntryListDto
            {
                Entries = entries.Select(e => new MyContestEntryDto
                {
                    EntryId = e.Id,
                    VoteCount = voteCounts.GetValueOrDefault(e.Id),
                    EnteredAt = e.CreatedAt,
                    VideoId = e.VideoId,
                    VideoTitle = e.Video.Title,
                    VideoUrl = e.Video.VideoUrl,
                    ThumbnailUrl = e.Video.ThumbnailUrl,
                    ContestId = e.ContestId,
                    ContestTitle = e.Contest.Title,
                    ContestCategory = e.Contest.Category,
                    ContestStartDate = e.Contest.StartDate,
                    ContestEndDate = e.Contest.EndDate
                }),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<MyContestEntryListDto> { Data = result, IsSuccess = true, Message = "Your contest entries retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ContestEntryDto>> AddEntryAsync(int userId, int contestId, CreateContestEntryRequest request)
        {
            var contest = await _contestRepo.GetByIdAsync(contestId);
            if (contest == null)
                return new Result<ContestEntryDto> { IsSuccess = false, Message = "Contest not found.", StatusCode = 404 };

            if (DateTime.UtcNow > contest.EndDate)
                return new Result<ContestEntryDto> { IsSuccess = false, Message = "This contest has ended.", StatusCode = 400 };

            var video = await _videoRepo.GetByIdAsync(request.VideoId!.Value);
            if (video == null || video.UserId != userId)
                return new Result<ContestEntryDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (video.Category != contest.Category)
                return new Result<ContestEntryDto> { IsSuccess = false, Message = "This video's category does not match the contest's category.", StatusCode = 400 };

            if (await _entryRepo.GetAsync(contestId, video.Id) != null)
                return new Result<ContestEntryDto> { IsSuccess = false, Message = "This video has already been entered into this contest.", StatusCode = 400 };

            var entry = new ContestEntry { ContestId = contestId, VideoId = video.Id };
            await _entryRepo.AddAsync(entry);
            await _entryRepo.SaveChangesAsync();

            var created = await _entryRepo.GetByIdWithDetailsAsync(entry.Id);
            return new Result<ContestEntryDto> { Data = ToDto(created!, 0, false), IsSuccess = true, Message = "Entry submitted successfully.", StatusCode = 201 };
        }

        public async Task<Result<object>> WithdrawEntryAsync(int userId, int contestId, int entryId, bool isAdmin)
        {
            var entry = await _entryRepo.GetByIdWithDetailsAsync(entryId);
            if (entry == null || entry.ContestId != contestId)
                return new Result<object> { IsSuccess = false, Message = "Entry not found.", StatusCode = 404 };

            if (!isAdmin)
            {
                if (entry.Video.UserId != userId)
                    return new Result<object> { IsSuccess = false, Message = "Entry not found.", StatusCode = 404 };

                if (DateTime.UtcNow > entry.Contest.EndDate)
                    return new Result<object> { IsSuccess = false, Message = "This contest has ended; entries can no longer be withdrawn.", StatusCode = 400 };
            }

            _entryRepo.Remove(entry);
            await _entryRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Entry withdrawn successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> ToggleVoteAsync(int userId, int contestId, int entryId)
        {
            var entry = await _entryRepo.GetByIdWithDetailsAsync(entryId);
            if (entry == null || entry.ContestId != contestId)
                return new Result<object> { IsSuccess = false, Message = "Entry not found.", StatusCode = 404 };

            var now = DateTime.UtcNow;
            if (now < entry.Contest.StartDate || now > entry.Contest.EndDate)
                return new Result<object> { IsSuccess = false, Message = "Voting is not open for this contest.", StatusCode = 400 };

            if (entry.Video.UserId == userId)
                return new Result<object> { IsSuccess = false, Message = "You cannot vote for your own entry.", StatusCode = 400 };

            var existing = await _voteRepo.GetAsync(entryId, userId);
            if (existing != null)
            {
                _voteRepo.Remove(existing);
                await _voteRepo.SaveChangesAsync();
                return new Result<object> { IsSuccess = true, Message = "Vote removed.", StatusCode = 200 };
            }

            await _voteRepo.AddAsync(new ContestVote { ContestEntryId = entryId, UserId = userId });
            await _voteRepo.SaveChangesAsync();
            return new Result<object> { IsSuccess = true, Message = "Vote cast.", StatusCode = 200 };
        }

        private static ContestDto ToDto(Contest contest, int entryCount) => new ContestDto
        {
            Id = contest.Id,
            Title = contest.Title,
            Description = contest.Description,
            Category = contest.Category,
            StartDate = contest.StartDate,
            EndDate = contest.EndDate,
            EntryCount = entryCount,
            CreatedAt = contest.CreatedAt
        };

        private static ContestEntryDto ToDto(ContestEntry entry, int voteCount, bool? isVoted) => new ContestEntryDto
        {
            Id = entry.Id,
            ContestId = entry.ContestId,
            VideoId = entry.VideoId,
            VideoTitle = entry.Video.Title,
            VideoUrl = entry.Video.VideoUrl,
            ThumbnailUrl = entry.Video.ThumbnailUrl,
            VoteCount = voteCount,
            IsVoted = isVoted,
            CreatedAt = entry.CreatedAt,
            Owner = new CommentAuthorDto
            {
                Id = entry.Video.User.Id,
                Username = entry.Video.User.Username,
                ProfileImageUrl = entry.Video.User.Profile?.ProfileImageUrl
            }
        };

    }
}