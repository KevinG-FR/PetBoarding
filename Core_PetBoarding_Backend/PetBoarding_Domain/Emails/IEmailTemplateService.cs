namespace PetBoarding_Domain.Emails;

public interface IEmailTemplateService
{
    Task<string> RenderAsync<T>(string templateName, T model, CancellationToken cancellationToken = default);
}