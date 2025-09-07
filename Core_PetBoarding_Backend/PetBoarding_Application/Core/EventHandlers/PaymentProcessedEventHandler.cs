using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Email.SendPaymentConfirmation;
using PetBoarding_Domain.Events;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Prestations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Application.Core.EventHandlers;

public sealed class PaymentProcessedEventHandler : IDomainEventHandler<PaymentProcessedEvent>
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPrestationRepository _prestationRepository;
    private readonly ILogger<PaymentProcessedEventHandler> _logger;

    public PaymentProcessedEventHandler(
        IMediator mediator,
        IUserRepository userRepository,
        IReservationRepository reservationRepository,
        IPrestationRepository prestationRepository,
        ILogger<PaymentProcessedEventHandler> logger)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _reservationRepository = reservationRepository;
        _prestationRepository = prestationRepository;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling PaymentProcessedEvent for payment {PaymentId} - User: {UserId}, Amount: {Amount}, Status: {Status}",
            domainEvent.PaymentId.Value, domainEvent.UserId.Value, domainEvent.Amount, domainEvent.Status);

        try
        {
            // Retrieve user information
            var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Could not retrieve user information for payment confirmation email. UserId: {UserId}",
                    domainEvent.UserId.Value);
                return;
            }

            // Retrieve reservation and service information if available
            string? reservationNumber = null;
            string? serviceName = null;

            if (domainEvent.ReservationId is not null)
            {
                var reservation = await _reservationRepository.GetByIdAsync(new ReservationId(domainEvent.ReservationId.Value), cancellationToken);
                if (reservation is not null)
                {
                    reservationNumber = reservation.Id.Value.ToString();
                    
                    // Get service name from prestation
                    var prestation = await _prestationRepository.GetByIdAsync(new PrestationId(new Guid(reservation.ServiceId)), cancellationToken);
                    serviceName = prestation?.Libelle;
                }
            }

            // Send payment confirmation email
            var sendPaymentConfirmationCommand = new SendPaymentConfirmationCommand(
                user.Email?.Value,
                $"{user.Firstname.Value} {user.Lastname.Value}",
                domainEvent.PaymentId.Value.ToString(),
                domainEvent.Amount,
                DateTime.UtcNow, // TODO: Get actual payment date from payment entity
                domainEvent.PaymentMethod,
                domainEvent.Status,
                reservationNumber,
                serviceName
            );

            var emailResult = await _mediator.Send(sendPaymentConfirmationCommand, cancellationToken);

            if (emailResult.IsFailed)
            {
                _logger.LogWarning("Failed to send payment confirmation email for payment {PaymentId}: {Error}",
                    domainEvent.PaymentId.Value, emailResult.Errors.FirstOrDefault()?.Message);
            }
            else
            {
                _logger.LogInformation("Payment confirmation email sent successfully for payment {PaymentId}",
                    domainEvent.PaymentId.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing payment confirmation email for payment {PaymentId}",
                domainEvent.PaymentId.Value);
        }
        
        _logger.LogInformation("PaymentProcessedEvent handled successfully for payment {PaymentId}", domainEvent.PaymentId.Value);
    }
}