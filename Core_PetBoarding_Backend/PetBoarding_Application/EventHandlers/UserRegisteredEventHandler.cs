using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Application.EventHandlers;

public sealed class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling UserRegisteredEvent for user {UserId} - {FirstName} {LastName} ({Email})",
            domainEvent.UserId.Value, domainEvent.FirstName, domainEvent.LastName, domainEvent.Email);

        // TODO: Implement business logic for user registration
        // Examples:
        // - Send welcome email
        // - Create user profile
        // - Setup default settings
        // - Log user registration metrics
        // - Trigger notification services

        await Task.CompletedTask;
        
        _logger.LogInformation("UserRegisteredEvent handled successfully for user {UserId}", domainEvent.UserId.Value);
    }
}