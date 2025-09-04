using MassTransit;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Events;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Infrastructure.Events.Consumers;

public sealed class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly IDomainEventHandler<PaymentProcessedEvent> _eventHandler;
    private readonly ILogger<PaymentProcessedEventConsumer> _logger;

    public PaymentProcessedEventConsumer(
        IDomainEventHandler<PaymentProcessedEvent> eventHandler,
        ILogger<PaymentProcessedEventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var domainEvent = context.Message;
        
        _logger.LogInformation("Consuming PaymentProcessedEvent {EventId} for payment {PaymentId} - Amount: {Amount}, Status: {Status}",
            domainEvent.EventId, domainEvent.PaymentId.Value, domainEvent.Amount, domainEvent.Status);

        try
        {
            await _eventHandler.HandleAsync(domainEvent, context.CancellationToken);
            
            _logger.LogInformation("Successfully consumed PaymentProcessedEvent {EventId}", domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PaymentProcessedEvent {EventId} for payment {PaymentId}",
                domainEvent.EventId, domainEvent.PaymentId.Value);
            throw;
        }
    }
}