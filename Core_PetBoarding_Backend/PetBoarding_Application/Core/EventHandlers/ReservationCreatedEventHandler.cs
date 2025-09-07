using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Email.SendReservationConfirmation;
using PetBoarding_Domain.Events;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Prestations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Application.Core.EventHandlers;

public sealed class ReservationCreatedEventHandler : IDomainEventHandler<ReservationCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IPetRepository _petRepository;
    private readonly IPrestationRepository _prestationRepository;
    private readonly ILogger<ReservationCreatedEventHandler> _logger;

    public ReservationCreatedEventHandler(
        IMediator mediator,
        IUserRepository userRepository,
        IPetRepository petRepository,
        IPrestationRepository prestationRepository,
        ILogger<ReservationCreatedEventHandler> logger)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _petRepository = petRepository;
        _prestationRepository = prestationRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ReservationCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ReservationCreatedEvent for reservation {ReservationId} - User: {UserId}, Pet: {PetId}, Amount: {TotalAmount}",
            domainEvent.ReservationId.Value, domainEvent.UserId.Value, domainEvent.PetId.Value, domainEvent.TotalAmount);

        try
        {
            // Retrieve detailed information for email
            var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
            var pet = await _petRepository.GetByIdAsync(domainEvent.PetId, cancellationToken);
            var prestation = await _prestationRepository.GetByIdAsync(domainEvent.PrestationId, cancellationToken);

            if (user == null || pet == null || prestation == null)
            {
                _logger.LogWarning("Could not retrieve required information for reservation confirmation email. User: {UserExists}, Pet: {PetExists}, Prestation: {PrestationExists}",
                    user != null, pet != null, prestation != null);
                return;
            }

            // Send reservation confirmation email
            var confirmationCommand = new SendReservationConfirmationCommand(
                user.Email?.Value,
                $"{user.Firstname?.Value} {user.Lastname?.Value}",
                pet.Name,
                prestation.Libelle,
                domainEvent.StartDate,
                domainEvent.EndDate,
                domainEvent.TotalAmount,
                domainEvent.ReservationId.Value.ToString(),
                new List<string>() // TODO: Add special instructions if available
            );

            var emailResult = await _mediator.Send(confirmationCommand, cancellationToken);

            if (emailResult.IsFailed)
            {
                _logger.LogWarning("Failed to send reservation confirmation email for reservation {ReservationId}: {Error}",
                    domainEvent.ReservationId.Value, emailResult.Errors.FirstOrDefault()?.Message);
            }
            else
            {
                _logger.LogInformation("Reservation confirmation email sent successfully for reservation {ReservationId}",
                    domainEvent.ReservationId.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing reservation confirmation email for reservation {ReservationId}",
                domainEvent.ReservationId.Value);
        }
        
        _logger.LogInformation("ReservationCreatedEvent handled successfully for reservation {ReservationId}", domainEvent.ReservationId.Value);
    }
}