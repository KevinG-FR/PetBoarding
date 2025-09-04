using FluentResults;

namespace PetBoarding_Domain.Emails;

public static class EmailErrors
{
    public static Error InvalidEmailAddress(string email) => 
        new Error($"The email address '{email}' is not valid");
        
    public static Error SendingFailed(string reason) => 
        new Error($"Failed to send email: {reason}");
        
    public static Error TemplateNotFound(string templateName) => 
        new Error($"Email template '{templateName}' not found");
        
    public static Error TemplateRenderFailed(string templateName, string reason) => 
        new Error($"Failed to render template '{templateName}': {reason}");
}