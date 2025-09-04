using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Email.SendWelcomeEmail;
using PetBoarding_Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Application.EventHandlers;

public sealed class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IMediator mediator, ILogger<UserRegisteredEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling UserRegisteredEvent for user {UserId} - {FirstName} {LastName} ({Email})",
            domainEvent.UserId.Value, domainEvent.FirstName, domainEvent.LastName, domainEvent.Email);

        // Send welcome email
        var welcomeEmailCommand = new SendWelcomeEmailCommand(
            domainEvent.Email,
            domainEvent.FirstName,
            domainEvent.LastName
        );

        var emailResult = await _mediator.Send(welcomeEmailCommand, cancellationToken);
        
        if (emailResult.IsFailed)
        {
            _logger.LogWarning("Failed to send welcome email to user {UserId}: {Error}",
                domainEvent.UserId.Value, emailResult.Errors.FirstOrDefault()?.Message);
        }
        else
        {
            _logger.LogInformation("Welcome email sent successfully to user {UserId}", domainEvent.UserId.Value);
        }
        
        _logger.LogInformation("UserRegisteredEvent handled successfully for user {UserId}", domainEvent.UserId.Value);
    }
}