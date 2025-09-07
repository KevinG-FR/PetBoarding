namespace PetBoarding_Application.Core.Reservations.ValidatePayment;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;

/// <summary>
/// Handler pour marquer une réservation comme payée (appelé par le système de panier)
/// </summary>
internal sealed class ValidatePaymentHandler : ICommandHandler<ValidatePaymentCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ValidatePaymentHandler> _logger;

    public ValidatePaymentHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ValidatePaymentHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Reservation>> Handle(
        ValidatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Récupérer la réservation
            if (!Guid.TryParse(request.ReservationId, out var reservationGuid))
            {
                return Result.Fail("Invalid ReservationId format");
            }

            var reservationId = new ReservationId(reservationGuid);
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservation is null)
            {
                return Result.Fail($"Reservation with ID {request.ReservationId} not found");
            }

            // 2. Vérifier que la réservation peut être marquée comme payée
            if (reservation.Status != ReservationStatus.Created)
            {
                return Result.Fail($"Cannot mark as paid a reservation with status {reservation.Status}");
            }

            // 3. Valider le montant (optionnel pour le moment)
            if (request.AmountPaid < 0)
            {
                return Result.Fail("Amount paid cannot be negative");
            }

            // 4. Marquer la réservation comme payée
            reservation.MarkAsPaid();
            
            if (request.AmountPaid > 0)
            {
                reservation.SetTotalPrice(request.AmountPaid);
            }

            // 5. Sauvegarder les modifications
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Reservation marked as paid {ReservationId}. Amount: {Amount}, Method: {Method}",
                request.ReservationId, request.AmountPaid, request.PaymentMethod);

            return Result.Ok(reservation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation during payment validation for reservation {ReservationId}", request.ReservationId);
            return Result.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating payment for reservation {ReservationId}", request.ReservationId);
            return Result.Fail($"Error validating payment: {ex.Message}");
        }
    }
}