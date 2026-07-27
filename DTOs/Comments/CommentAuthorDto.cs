namespace TalentShowcase.Api.DTOs.Comments
{
    public class CommentAuthorDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}