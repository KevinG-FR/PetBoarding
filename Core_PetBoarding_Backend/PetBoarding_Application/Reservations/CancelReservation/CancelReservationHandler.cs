namespace PetBoarding_Application.Reservations.CancelReservation;

using FluentResults;

using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;

internal sealed class CancelReservationHandler : ICommandHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelReservationHandler> _logger;

    public CancelReservationHandler(
        IReservationRepository reservationRepository,
        IPlanningRepository planningRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _planningRepository = planningRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(request.Id, out var guidId))
            {
                return Result.Fail("Invalid reservation ID");
            }

            var reservationId = new ReservationId(guidId);
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservation is null)
            {
                return Result.Fail("Reservation not found");
            }

            _logger.LogInformation("Cancelling reservation {ReservationId} with status {Status}", request.Id, reservation.Status);

            // 1. Vérifier si la réservation peut être annulée
            if (!reservation.IsActivelyReserved())
            {
                return Result.Fail($"Cannot cancel reservation with status {reservation.Status}");
            }

            // 2. Obtenir le planning pour libérer les créneaux
            if (!Guid.TryParse(reservation.ServiceId, out var serviceGuid))
            {
                return Result.Fail("Invalid ServiceId format");
            }

            var prestationId = new PrestationId(serviceGuid);
            var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);
            
            if (planning is null)
            {
                _logger.LogError("No planning found for reservation {ReservationId} with ServiceId {ServiceId}", request.Id, reservation.ServiceId);
                return Result.Fail("Planning not found for this service");
            }

            // 3. Libérer tous les créneaux spécifiques de la réservation
            var releasedSlots = 0;
            var failedReleases = 0;
            var activeSlotIds = reservation.GetActiveReservedSlotIds().ToList();
            
            foreach (var slotId in activeSlotIds)
            {
                try
                {
                    // Libérer dans le planning
                    var availableSlot = planning.GetSlotById(new AvailableSlotId(slotId));
                    if (availableSlot is not null)
                    {
                        availableSlot.CancelReservation(1);
                        releasedSlots++;
                        _logger.LogDebug("Released slot {SlotId} for reservation {ReservationId}", slotId, request.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Slot {SlotId} not found in planning for reservation {ReservationId}", slotId, request.Id);
                        failedReleases++;
                    }
                    
                    // Marquer comme libéré dans la réservation
                    reservation.ReleaseReservedSlot(slotId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to release slot {SlotId} for reservation {ReservationId}", slotId, request.Id);
                    failedReleases++;
                }
            }

            if (failedReleases > 0)
            {
                _logger.LogWarning("Failed to release {FailedCount} slots out of {TotalSlots} for reservation {ReservationId}", 
                    failedReleases, releasedSlots + failedReleases, request.Id);
            }

            // 4. Annuler la réservation (change le statut)
            reservation.Cancel();

            // 5. Sauvegarder toutes les modifications (transaction atomique)
            var updatedReservation = await _reservationRepository.UpdateAsync(reservation, cancellationToken);
            if (updatedReservation is null)
            {
                return Result.Fail("Error occurred while cancelling the reservation");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully cancelled reservation {ReservationId} and released {SlotCount} slots", 
                request.Id, releasedSlots);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling reservation: {ex.Message}");
        }
    }
}
