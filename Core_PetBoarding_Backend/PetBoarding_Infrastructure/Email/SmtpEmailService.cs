using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetBoarding_Domain.Emails;

namespace PetBoarding_Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailConfiguration> config, ILogger<SmtpEmailService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending email to {ToEmail} with subject: {Subject}", 
                message.ToEmail, message.Subject);

            using var client = new SmtpClient(_config.SmtpHost, _config.SmtpPort)
            {
                EnableSsl = _config.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            // Only set credentials if username/password are provided
            if (!string.IsNullOrEmpty(_config.Username) && !string.IsNullOrEmpty(_config.Password))
            {
                client.Credentials = new NetworkCredential(_config.Username, _config.Password);
            }

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_config.FromEmail, _config.FromName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml
            };

            mailMessage.To.Add(new MailAddress(message.ToEmail, message.ToName));

            if (!string.IsNullOrEmpty(_config.ReplyToEmail))
            {
                mailMessage.ReplyToList.Add(new MailAddress(_config.ReplyToEmail));
            }

            // Add attachments if any
            foreach (var attachment in message.Attachments)
            {
                var stream = new MemoryStream(attachment.Content);
                var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                mailMessage.Attachments.Add(mailAttachment);
            }

            await client.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully to {ToEmail}", message.ToEmail);
            return EmailResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}: {ErrorMessage}", 
                message.ToEmail, ex.Message);
            return EmailResult.Failure(ex.Message, ex);
        }
    }
}