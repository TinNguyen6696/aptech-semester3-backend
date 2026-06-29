namespace TalentShowcase.Api.Models
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Mes { get; set; }
    }
}
