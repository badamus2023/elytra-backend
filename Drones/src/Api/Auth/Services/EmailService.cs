using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Drones.src.Api.Auth.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string token)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var baseUrl = emailSettings["BaseUrl"];
            var verifyUrl = $"{baseUrl}/auth/verify-email?token={token}";

            await SendEmailAsync(
                to: toEmail,
                subject: "Verify Your Email",
                body: $"""
                <h2>Email Verification</h2>
                <p>Click the link below to verify your email address.</p>
                <p>This link expires in <strong>24 hours</strong>.</p>
                <a href="{verifyUrl}">Verify Email</a>
                <p>If you did not create an account, ignore this email.</p>
                """
            );
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string token)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var baseUrl = emailSettings["BaseUrl"];
            var resetUrl = $"{baseUrl}/auth/reset-password?token={token}";

            await SendEmailAsync(
                to: toEmail,
                subject: "Password Reset Request",
                body: $"""
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password.</p>
                <p>This link expires in <strong>1 hour</strong>.</p>
                <a href="{resetUrl}">Reset Password</a>
                <p>If you did not request a password reset, ignore this email.</p>
                """
            );
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]
            ));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                emailSettings["SmtpHost"],
                int.Parse(emailSettings["SmtpPort"]
                    ?? throw new InvalidOperationException("EmailSettings:SmtpPort is not configured")),
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                emailSettings["SmtpUser"],
                emailSettings["SmtpPassword"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
