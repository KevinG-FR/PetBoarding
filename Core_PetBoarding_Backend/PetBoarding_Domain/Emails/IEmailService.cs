namespace PetBoarding_Domain.Emails;

public interface IEmailService
{
    Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}