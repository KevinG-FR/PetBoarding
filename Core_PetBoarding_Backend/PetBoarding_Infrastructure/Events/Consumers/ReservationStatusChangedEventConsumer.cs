using MassTransit;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class ReservationStatusChangedEventConsumer : IConsumer<ReservationStatusChangedEvent>
{
    private readonly IDomainEventHandler<ReservationStatusChangedEvent> _eventHandler;
    private readonly ILogger<ReservationStatusChangedEventConsumer> _logger;

    public ReservationStatusChangedEventConsumer(
        IDomainEventHandler<ReservationStatusChangedEvent> eventHandler,
        ILogger<ReservationStatusChangedEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReservationStatusChangedEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming ReservationStatusChangedEvent {EventId} for reservation {ReservationId} - Status: {OldStatus} -> {NewStatus}",
            domainEvent.EventId, domainEvent.ReservationId.Value, domainEvent.OldStatus, domainEvent.NewStatus);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed ReservationStatusChangedEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming ReservationStatusChangedEvent {EventId} for reservation {ReservationId}",
                domainEvent.EventId, domainEvent.ReservationId.Value);
            throw;
        }
    }
}