using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Account;
using PetBoarding_Application.EventHandlers;
using PetBoarding_Application.Planning.ReleaseSlots;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Events;

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

        // Event Handlers
        services.AddScoped<IDomainEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
        services.AddScoped<IDomainEventHandler<UserProfileUpdatedEvent>, UserProfileUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<PetRegisteredEvent>, PetRegisteredEventHandler>();
        services.AddScoped<IDomainEventHandler<ReservationCreatedEvent>, ReservationCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ReservationStatusChangedEvent>, ReservationStatusChangedEventHandler>();
        services.AddScoped<IDomainEventHandler<PaymentProcessedEvent>, PaymentProcessedEventHandler>();

        return services;
    }
}
