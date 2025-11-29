using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MobileSparePartsManagement.Infrastructure.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string resetUrl)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        var smtpHost = smtpSettings["Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
        var smtpUsername = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
        var smtpPassword = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
        var fromEmail = smtpSettings["FromEmail"] ?? smtpUsername;
        var fromName = smtpSettings["FromName"] ?? "Mobile Spare Parts Management";

        var resetLink = $"{resetUrl}?token={Uri.EscapeDataString(resetToken)}";

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = "Password Reset Request",
            Body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password. Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request this, please ignore this email.</p>
                </body>
                </html>
            ",
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}