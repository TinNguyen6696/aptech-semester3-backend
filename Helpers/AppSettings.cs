namespace TalentShowcase.Api.Helpers
{
    public class AppSettings
    {
        // Base URL of the React/Vite frontend. Verification and reset links
        // point here; the frontend then calls our JSON API with the token.
        public string FrontendBaseUrl { get; set; } = null!;
        public int EmailVerificationTokenHours { get; set; } = 24;
        public int PasswordResetTokenHours { get; set; } = 1;
    }
}
