namespace PetBoarding_Application.Core.TaskWorkerProcess.ProcessExpiredBaskets;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Planning.ReleaseSlots;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

/// <summary>
/// Handler pour traiter les paniers expirés et libérer leurs créneaux automatiquement
/// </summary>
internal sealed class ProcessExpiredBasketsCommandHandler : ICommandHandler<ProcessExpiredBasketsCommand, int>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IReleaseSlotService _releaseSlotService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessExpiredBasketsCommandHandler> _logger;

    public ProcessExpiredBasketsCommandHandler(
        IBasketRepository basketRepository,
        IPlanningRepository planningRepository,
        IReleaseSlotService releaseSlotService,
        IUnitOfWork unitOfWork,
        ILogger<ProcessExpiredBasketsCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _planningRepository = planningRepository;
        _releaseSlotService = releaseSlotService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        ProcessExpiredBasketsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing expired baskets older than {ExpirationMinutes} minutes...", request.ExpirationMinutes);
            
            // 1. Récupérer tous les paniers expirés
            var expiredBaskets = await _basketRepository.GetExpiredBaskets(request.ExpirationMinutes, cancellationToken);
            var basketsList = expiredBaskets.ToList();
            
            var processedCount = 0;
            var totalReleasedSlots = 0;
            
            foreach (var basket in basketsList)
            {
                try
                {
                    _logger.LogDebug("Processing expired basket {BasketId} for user {UserId} created at {CreatedAt}", 
                        basket.Id.Value, basket.UserId.Value, basket.CreatedAt);
                    
                    var releasedSlotsForBasket = 0;
                    
                    // 2. Pour chaque réservation dans le panier, libérer les créneaux
                    foreach (var basketItem in basket.Items)
                    {
                        try
                        {
                            var reservation = basketItem.Reservation;
                            if (reservation == null)
                            {
                                _logger.LogWarning("No reservation found for basket item in basket {BasketId}", basket.Id.Value);
                                continue;
                            }

                            // 3. Obtenir le planning correspondant à la prestation
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

                            // 4. Libérer les créneaux de la réservation
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
                                        releasedSlotsForBasket++;
                                        _logger.LogDebug("Released slot {SlotId} from expired basket {BasketId}", 
                                            slotId, basket.Id.Value);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Slot {SlotId} not found in planning for expired basket {BasketId}", 
                                            slotId, basket.Id.Value);
                                    }
                                    
                                    // Marquer comme libéré dans la réservation
                                    reservation.ReleaseReservedSlot(slotId);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Failed to release slot {SlotId} for expired basket {BasketId}", 
                                        slotId, basket.Id.Value);
                                }
                            }

                            // 5. Annuler la réservation
                            reservation.Cancel();
                            _logger.LogDebug("Cancelled reservation {ReservationId} from expired basket {BasketId}", 
                                reservation.Id.Value, basket.Id.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing basket item for expired basket {BasketId}", basket.Id.Value);
                            // Continue avec le prochain item
                        }
                    }
                    
                    // 6. Annuler le panier
                    basket.Cancel();
                    
                    _logger.LogInformation("Processed expired basket {BasketId}: cancelled {ItemCount} items, released {SlotCount} slots", 
                        basket.Id.Value, basket.Items.Count, releasedSlotsForBasket);
                    
                    processedCount++;
                    totalReleasedSlots += releasedSlotsForBasket;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired basket {BasketId}", basket.Id.Value);
                    // Continue avec le prochain panier
                }
            }
            
            // 7. Sauvegarder toutes les modifications
            if (processedCount > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully processed {Count} expired baskets, released {SlotCount} slots total", 
                    processedCount, totalReleasedSlots);
            }
            else
            {
                _logger.LogInformation("No expired baskets to process");
            }
            
            return Result.Ok(processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during expired baskets processing");
            return Result.Fail($"Failed to process expired baskets: {ex.Message}");
        }
    }
}