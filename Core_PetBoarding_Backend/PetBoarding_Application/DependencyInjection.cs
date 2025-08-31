using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Application.Account;
using PetBoarding_Application.Planning.ReleaseSlots;

using PetBoarding_Domain.Accounts;

namespace PetBoarding_Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblies(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IReleaseSlotService, ReleaseSlotService>();

        return services;
    }
}
