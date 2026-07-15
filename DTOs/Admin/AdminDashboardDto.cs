namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int MemberCount { get; set; }
        public int MentorCount { get; set; }
        public int RecruiterCount { get; set; }
        public int AdminCount { get; set; }
        public int TotalVideos { get; set; }
        public int TotalContests { get; set; }
        public int TotalOpportunities { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int NewUsersLast7Days { get; set; }
        public int NewVideosLast7Days { get; set; }
    }
}