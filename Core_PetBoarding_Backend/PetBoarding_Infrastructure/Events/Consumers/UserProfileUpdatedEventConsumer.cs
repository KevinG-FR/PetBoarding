using MassTransit;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class UserProfileUpdatedEventConsumer : IConsumer<UserProfileUpdatedEvent>
{
    private readonly IDomainEventHandler<UserProfileUpdatedEvent> _eventHandler;
    private readonly ILogger<UserProfileUpdatedEventConsumer> _logger;

    public UserProfileUpdatedEventConsumer(
        IDomainEventHandler<UserProfileUpdatedEvent> eventHandler,
        ILogger<UserProfileUpdatedEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserProfileUpdatedEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming UserProfileUpdatedEvent {EventId} for user {UserId}",
            domainEvent.EventId, domainEvent.UserId.Value);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed UserProfileUpdatedEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming UserProfileUpdatedEvent {EventId} for user {UserId}",
                domainEvent.EventId, domainEvent.UserId.Value);
            throw;
        }
    }
}