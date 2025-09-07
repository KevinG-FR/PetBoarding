using MassTransit;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class ReservationCreatedEventConsumer : IConsumer<ReservationCreatedEvent>
{
    private readonly IDomainEventHandler<ReservationCreatedEvent> _eventHandler;
    private readonly ILogger<ReservationCreatedEventConsumer> _logger;

    public ReservationCreatedEventConsumer(
        IDomainEventHandler<ReservationCreatedEvent> eventHandler,
        ILogger<ReservationCreatedEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReservationCreatedEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming ReservationCreatedEvent {EventId} for reservation {ReservationId}",
            domainEvent.EventId, domainEvent.ReservationId.Value);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed ReservationCreatedEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming ReservationCreatedEvent {EventId} for reservation {ReservationId}",
                domainEvent.EventId, domainEvent.ReservationId.Value);
            throw;
        }
    }
}