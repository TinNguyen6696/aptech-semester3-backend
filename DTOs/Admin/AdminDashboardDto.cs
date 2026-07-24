namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int BannedUsers { get; set; }
        public int MemberCount { get; set; }
        public int MentorCount { get; set; }
        public int RecruiterCount { get; set; }
        public int AdminCount { get; set; }
        public int TotalVideos { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalContests { get; set; }
        public int EndedContests { get; set; }
        public int TotalOpportunities { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int TotalMessages { get; set; }
        public int NewUsersLast7Days { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int NewVideosLast7Days { get; set; }
        public IEnumerable<AdminUserDto> RecentUsers { get; set; } = new List<AdminUserDto>();
    }
}
