using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Communities;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class CommunityService : ICommunityService
    {
        private const int MaxPageSize = 10;

        private readonly IGenericRepository<Community> _communityRepo;
        private readonly ICommunityPostRepository _postRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly ICommentRepository _commentRepo;

        public CommunityService(
            IGenericRepository<Community> communityRepo,
            ICommunityPostRepository postRepo,
            ILikeRepository likeRepo,
            ICommentRepository commentRepo)
        {
            _communityRepo = communityRepo;
            _postRepo = postRepo;
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
        }

        public async Task<Result<IEnumerable<CommunityDto>>> GetCommunitiesAsync()
        {
            var communities = (await _communityRepo.GetAllAsync()).ToList();
            var postCounts = await _postRepo.CountByCommunityIdsAsync(communities.Select(c => c.Id));

            var dtos = communities
                .OrderBy(c => c.Id)
                .Select(c => ToDto(c, postCounts.GetValueOrDefault(c.Id)));

            return new Result<IEnumerable<CommunityDto>> { Data = dtos, IsSuccess = true, Message = "Communities retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<CommunityDto>> GetCommunityByIdAsync(int id)
        {
            var community = await _communityRepo.GetByIdAsync(id);
            if (community == null)
                return new Result<CommunityDto> { IsSuccess = false, Message = "Community not found.", StatusCode = 404 };

            var postCount = await _postRepo.CountByCommunityIdAsync(id);
            return new Result<CommunityDto> { Data = ToDto(community, postCount), IsSuccess = true, Message = "Community retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<CommunityPostListDto>> GetCommunityPostsAsync(int communityId, int page, int pageSize)
        {
            var community = await _communityRepo.GetByIdAsync(communityId);
            if (community == null)
                return new Result<CommunityPostListDto> { IsSuccess = false, Message = "Community not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<CommunityPostListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<CommunityPostListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _postRepo.CountByCommunityIdAsync(communityId);
            var posts = (await _postRepo.GetByCommunityIdAsync(communityId, page, pageSize)).ToList();

            var postIds = posts.Select(p => p.Id);
            var likeCounts = await _likeRepo.CountByReferenceIdsAsync(ReferenceTypes.CommunityPost, postIds);
            var commentCounts = await _commentRepo.CountByReferenceIdsAsync(ReferenceTypes.CommunityPost, postIds);

            var result = new CommunityPostListDto
            {
                Posts = posts.Select(p => ToDto(p, likeCounts.GetValueOrDefault(p.Id), commentCounts.GetValueOrDefault(p.Id))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<CommunityPostListDto> { Data = result, IsSuccess = true, Message = "Posts retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<CommunityPostDto>> AddCommunityPostAsync(int userId, int communityId, CreateCommunityPostRequest request)
        {
            var community = await _communityRepo.GetByIdAsync(communityId);
            if (community == null)
                return new Result<CommunityPostDto> { IsSuccess = false, Message = "Community not found.", StatusCode = 404 };

            var post = new CommunityPost
            {
                CommunityId = communityId,
                UserId = userId,
                Content = request.Content
            };

            await _postRepo.AddAsync(post);
            await _postRepo.SaveChangesAsync();

            var created = await _postRepo.GetByIdWithUserAsync(post.Id);
            return new Result<CommunityPostDto> { Data = ToDto(created!, 0, 0), IsSuccess = true, Message = "Post added successfully.", StatusCode = 201 };
        }

        public async Task<Result<CommunityPostDto>> UpdateCommunityPostAsync(int userId, int postId, UpdateCommunityPostRequest request)
        {
            var post = await _postRepo.GetByIdWithUserAsync(postId);
            if (post == null || post.UserId != userId)
                return new Result<CommunityPostDto> { IsSuccess = false, Message = "Post not found.", StatusCode = 404 };

            post.Content = request.Content;
            _postRepo.Update(post);
            await _postRepo.SaveChangesAsync();

            var likeCount = await _likeRepo.CountByReferenceAsync(ReferenceTypes.CommunityPost, postId);
            var commentCount = await _commentRepo.CountByReferenceAsync(ReferenceTypes.CommunityPost, postId);

            return new Result<CommunityPostDto> { Data = ToDto(post, likeCount, commentCount), IsSuccess = true, Message = "Post updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteCommunityPostAsync(int userId, int postId, bool isAdmin)
        {
            var post = await _postRepo.GetByIdAsync(postId);
            if (post == null || (post.UserId != userId && !isAdmin))
                return new Result<object> { IsSuccess = false, Message = "Post not found.", StatusCode = 404 };

            _postRepo.Remove(post);
            await _postRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Post deleted successfully.", StatusCode = 200 };
        }

        private static CommunityDto ToDto(Community community, int postCount) => new CommunityDto
        {
            Id = community.Id,
            Name = community.Name,
            Description = community.Description,
            Category = community.Category,
            PostCount = postCount
        };

        private static CommunityPostDto ToDto(CommunityPost post, int likeCount, int commentCount) => new CommunityPostDto
        {
            Id = post.Id,
            CommunityId = post.CommunityId,
            Content = post.Content,
            LikeCount = likeCount,
            CommentCount = commentCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Author = new CommentAuthorDto
            {
                Id = post.User.Id,
                Username = post.User.Username,
                ProfileImageUrl = post.User.Profile?.ProfileImageUrl
            }
        };
    }
}