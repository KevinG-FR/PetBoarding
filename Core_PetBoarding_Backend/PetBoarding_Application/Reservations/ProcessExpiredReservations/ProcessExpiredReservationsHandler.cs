namespace PetBoarding_Application.Reservations.ProcessExpiredReservations;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;

/// <summary>
/// Handler pour traiter les réservations expirées et libérer les créneaux automatiquement
/// </summary>
internal sealed class ProcessExpiredReservationsHandler : ICommandHandler<ProcessExpiredReservationsCommand, int>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessExpiredReservationsHandler> _logger;

    public ProcessExpiredReservationsHandler(
        IReservationRepository reservationRepository,
        IPlanningRepository planningRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessExpiredReservationsHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _planningRepository = planningRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        ProcessExpiredReservationsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing expired reservations...");
            
            // 1. Récupérer toutes les réservations créées mais expirées
            var expiredReservations = await _reservationRepository.GetExpiredCreatedReservationsAsync(cancellationToken);
            
            var processedCount = 0;
            
            foreach (var reservation in expiredReservations)
            {
                try
                {
                    // 2. Skip expiry check - payment expiry logic has been removed
                    // All reservations returned by GetExpiredCreatedReservationsAsync should be processed
                    
                    // 3. Obtenir le planning correspondant
                    if (!Guid.TryParse(reservation.ServiceId, out var serviceGuid))
                    {
                        _logger.LogError("Invalid ServiceId format for reservation {ReservationId}: {ServiceId}", 
                            reservation.Id.Value, reservation.ServiceId);
                        continue;
                    }
                    
                    var prestationId = new PrestationId(serviceGuid);
                    var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);
                    
                    if (planning == null)
                    {
                        _logger.LogError("No planning found for reservation {ReservationId} with ServiceId {ServiceId}", 
                            reservation.Id.Value, reservation.ServiceId);
                        continue;
                    }
                    
                    // 4. Libérer les créneaux spécifiques de la réservation expirée
                    var releasedSlots = 0;
                    var activeSlotIds = reservation.GetActiveReservedSlotIds().ToList();
                    
                    foreach (var slotId in activeSlotIds)
                    {
                        try
                        {
                            // Libérer dans le planning
                            var availableSlot = planning.GetSlotById(new AvailableSlotId(slotId));
                            if (availableSlot != null)
                            {
                                availableSlot.CancelReservation(1);
                                releasedSlots++;
                                _logger.LogDebug("Released expired slot {SlotId} for reservation {ReservationId}", 
                                    slotId, reservation.Id.Value);
                            }
                            else
                            {
                                _logger.LogWarning("Expired slot {SlotId} not found in planning for reservation {ReservationId}", 
                                    slotId, reservation.Id.Value);
                            }
                            
                            // Marquer comme libéré dans la réservation
                            reservation.ReleaseReservedSlot(slotId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to release expired slot {SlotId} for reservation {ReservationId}", 
                                slotId, reservation.Id.Value);
                        }
                    }
                    
                    // 5. Marquer la réservation comme annulée (plus d'expiration automatique)
                    reservation.Cancel();
                    
                    _logger.LogInformation("Processed expired reservation {ReservationId}: released {SlotCount} slots", 
                        reservation.Id.Value, releasedSlots);
                    
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired reservation {ReservationId}", reservation.Id.Value);
                    // Continue with next reservation instead of failing completely
                }
            }
            
            // 6. Sauvegarder toutes les modifications
            if (processedCount > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully processed {Count} expired reservations", processedCount);
            }
            else
            {
                _logger.LogInformation("No expired reservations to process");
            }
            
            return Result.Ok(processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during expired reservations processing");
            return Result.Fail($"Failed to process expired reservations: {ex.Message}");
        }
    }
}