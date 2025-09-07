using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.EventHandlers;
using PetBoarding_Application.Core.Planning.ReleaseSlots;

using PetBoarding_Domain.Events;

namespace PetBoarding_Application.Core.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblies(assembly));

        services.AddValidatorsFromAssembly(assembly);

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
