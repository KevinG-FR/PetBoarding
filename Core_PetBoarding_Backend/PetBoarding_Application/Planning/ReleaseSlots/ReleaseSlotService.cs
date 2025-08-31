namespace PetBoarding_Application.Planning.ReleaseSlots;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;

/// <summary>
/// Service pour la libération des créneaux de réservation
/// </summary>
internal sealed class ReleaseSlotService : IReleaseSlotService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReleaseSlotService> _logger;

    public ReleaseSlotService(
        IReservationRepository reservationRepository,
        IPlanningRepository planningRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReleaseSlotService> logger)
    {
        _reservationRepository = reservationRepository;
        _planningRepository = planningRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> ReleaseReservationSlotsAsync(
        ReservationId reservationId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Récupérer la réservation
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                return Result.Fail<int>("Reservation not found");
            }

            // 2. Obtenir le planning pour libérer les créneaux
            if (!Guid.TryParse(reservation.ServiceId, out var serviceGuid))
            {
                return Result.Fail<int>("Invalid ServiceId format");
            }

            var prestationId = new PrestationId(serviceGuid);
            var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);
            
            if (planning is null)
            {
                _logger.LogError("No planning found for reservation {ReservationId} with ServiceId {ServiceId}", 
                    reservationId.Value, reservation.ServiceId);
                return Result.Fail<int>("Planning not found for this service");
            }

            // 3. Libérer tous les créneaux spécifiques de la réservation
            var releasedSlots = 0;
            var failedReleases = 0;
            var activeSlotIds = reservation.GetActiveReservedSlotIds().ToList();
            
            _logger.LogInformation("Releasing {SlotCount} slots for reservation {ReservationId}", 
                activeSlotIds.Count, reservationId.Value);

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
                        _logger.LogDebug("Released slot {SlotId} for reservation {ReservationId}", 
                            slotId, reservationId.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Slot {SlotId} not found in planning for reservation {ReservationId}", 
                            slotId, reservationId.Value);
                        failedReleases++;
                    }

                    // Les slots seront marqués comme libérés via ReleaseReservedSlots() après la boucle
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error releasing slot {SlotId} for reservation {ReservationId}", 
                        slotId, reservationId.Value);
                    failedReleases++;
                }
            }

            // 4. Marquer tous les slots comme libérés dans la réservation
            reservation.ReleaseAllReservedSlots();

            // 5. Sauvegarder les changements si au moins un créneau a été libéré
            if (releasedSlots > 0)
            {
                await _planningRepository.UpdateAsync(planning, cancellationToken);
                await _reservationRepository.UpdateAsync(reservation, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully released {ReleasedSlots} slots for reservation {ReservationId}. Failed: {FailedReleases}", 
                    releasedSlots, reservationId.Value, failedReleases);
            }
            else if (failedReleases > 0)
            {
                _logger.LogError("Failed to release any slots for reservation {ReservationId}. All {FailedReleases} attempts failed", 
                    reservationId.Value, failedReleases);
                return Result.Fail<int>($"Failed to release any slots. {failedReleases} failures occurred");
            }

            return Result.Ok(releasedSlots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error releasing slots for reservation {ReservationId}", reservationId.Value);
            return Result.Fail<int>($"Unexpected error: {ex.Message}");
        }
    }
}