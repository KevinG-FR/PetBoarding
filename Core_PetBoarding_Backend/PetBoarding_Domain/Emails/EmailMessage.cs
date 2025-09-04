namespace PetBoarding_Domain.Emails;

public sealed class EmailMessage
{
    public string ToEmail { get; init; } = string.Empty;
    public string ToName { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public bool IsHtml { get; init; } = true;
    public List<EmailAttachment> Attachments { get; init; } = new();
}