using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Application.Web.Account;
using PetBoarding_Domain.Accounts;

namespace PetBoarding_Application.Web.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationWeb(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Register MediatR for Web-specific handlers (authentication)
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblies(assembly));

        // Web-specific services (authentication)
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}