using MassTransit;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class PetRegisteredEventConsumer : IConsumer<PetRegisteredEvent>
{
    private readonly IDomainEventHandler<PetRegisteredEvent> _eventHandler;
    private readonly ILogger<PetRegisteredEventConsumer> _logger;

    public PetRegisteredEventConsumer(
        IDomainEventHandler<PetRegisteredEvent> eventHandler,
        ILogger<PetRegisteredEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PetRegisteredEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming PetRegisteredEvent {EventId} for pet {PetId} owned by user {OwnerId}",
            domainEvent.EventId, domainEvent.PetId.Value, domainEvent.OwnerId.Value);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed PetRegisteredEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PetRegisteredEvent {EventId} for pet {PetId}",
                domainEvent.EventId, domainEvent.PetId.Value);
            throw;
        }
    }
}