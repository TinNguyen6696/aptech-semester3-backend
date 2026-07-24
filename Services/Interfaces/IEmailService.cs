namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string username, string verifyUrl);
        Task SendPasswordResetAsync(string toEmail, string username, string resetUrl);
        Task SendContestWinnerAsync(string toEmail, string username, string contestTitle);
    }
}
