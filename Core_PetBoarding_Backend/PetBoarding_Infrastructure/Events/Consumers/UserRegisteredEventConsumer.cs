using MassTransit;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IDomainEventHandler<UserRegisteredEvent> _eventHandler;
    private readonly ILogger<UserRegisteredEventConsumer> _logger;

    public UserRegisteredEventConsumer(
        IDomainEventHandler<UserRegisteredEvent> eventHandler,
        ILogger<UserRegisteredEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming UserRegisteredEvent {EventId} for user {UserId}",
            domainEvent.EventId, domainEvent.UserId.Value);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed UserRegisteredEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming UserRegisteredEvent {EventId} for user {UserId}",
                domainEvent.EventId, domainEvent.UserId.Value);
            throw;
        }
    }
}