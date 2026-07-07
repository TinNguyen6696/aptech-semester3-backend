using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TalentShowcase.Api.Helpers;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;

        public EmailService(SmtpSettings smtp)
        {
            _smtp = smtp;
        }

        public Task SendEmailVerificationAsync(string toEmail, string username, string verifyUrl)
        {
            var body =
                $"<p>Hi {username},</p>" +
                "<p>Thanks for signing up to Talent Showcase. Please confirm your email address by clicking the link below:</p>" +
                $"<p><a href=\"{verifyUrl}\">Verify my email</a></p>" +
                "<p>If you didn't create this account, you can safely ignore this email.</p>";

            return SendAsync(toEmail, "Verify your Talent Showcase email", body);
        }

        public Task SendPasswordResetAsync(string toEmail, string username, string resetUrl)
        {
            var body =
                $"<p>Hi {username},</p>" +
                "<p>We received a request to reset your Talent Showcase password. Click the link below to choose a new one:</p>" +
                $"<p><a href=\"{resetUrl}\">Reset my password</a></p>" +
                "<p>This link will expire soon. If you didn't request this, you can safely ignore this email.</p>";

            return SendAsync(toEmail, "Reset your Talent Showcase password", body);
        }

        private async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            var socketOption = _smtp.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

            await client.ConnectAsync(_smtp.Host, _smtp.Port, socketOption);
            if (!string.IsNullOrEmpty(_smtp.Username))
                await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
