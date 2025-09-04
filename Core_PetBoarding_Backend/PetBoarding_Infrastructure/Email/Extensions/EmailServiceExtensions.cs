using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetBoarding_Domain.Emails;
using PetBoarding_Infrastructure.Email;

namespace PetBoarding_Infrastructure.Email.Extensions;

public static class EmailServiceExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure EmailConfiguration
        services.Configure<EmailConfiguration>(configuration.GetSection(EmailConfiguration.SectionName));
        
        // Register email services
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IEmailTemplateService, SimpleTemplateService>();
        
        return services;
    }
}