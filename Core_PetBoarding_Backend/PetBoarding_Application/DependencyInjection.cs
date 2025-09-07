using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.EventHandlers;
using PetBoarding_Application.Core.Planning.ReleaseSlots;
using PetBoarding_Application.Web.Account;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Events;

namespace PetBoarding_Application;

public static class DependencyInjection
{
    /// <summary>
    /// Add core business logic services (for TaskWorker and API)
    /// </summary>
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        var assembly = typeof(PetBoarding_Application.Core.Abstractions.IDomainEventHandler<>).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(assembly);
            configuration.TypeEvaluator = type => !type.Namespace?.Contains(".Web.") ?? true;
        });

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

    /// <summary>
    /// Add web-specific services (authentication, JWT) for API only
    /// </summary>
    public static IServiceCollection AddApplicationWeb(this IServiceCollection services)
    {
        var assembly = typeof(PetBoarding_Application.Web.Abstractions.IJwtProvider).Assembly;

        // Register MediatR for Web-specific handlers only (authentication)
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(assembly);
            configuration.TypeEvaluator = type => type.Namespace?.Contains(".Web.") ?? false;
        });

        // Web-specific services (authentication)
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}