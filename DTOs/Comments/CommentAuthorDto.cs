namespace TalentShowcase.Api.DTOs.Comments
{
    public class CommentAuthorDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
    }
}