namespace PetBoarding_Application.Baskets.ClearBasket;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Planning.ReleaseSlots;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;

internal sealed class ClearBasketCommandHandler : ICommandHandler<ClearBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IReleaseSlotService _releaseSlotService;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClearBasketCommandHandler> _logger;

    public ClearBasketCommandHandler(
        IBasketRepository basketRepository,
        IReleaseSlotService releaseSlotService,
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ClearBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _releaseSlotService = releaseSlotService;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basketId = new BasketId(request.BasketId);

        _logger.LogInformation("Clearing basket {BasketId}", request.BasketId);

        var basket = await _basketRepository.GetByIdWithItemsAsync(basketId, cancellationToken);
        if (basket is null)
            return Result.Fail("Basket not found");

        // Libérer les créneaux de toutes les réservations du panier avant de le vider
        var reservationIds = basket.GetReservationIds().ToList();
        var totalReleasedSlots = 0;
        var releaseErrors = new List<string>();

        _logger.LogInformation("Releasing slots for {ReservationCount} reservations in basket {BasketId}", 
            reservationIds.Count, request.BasketId);

        foreach (var reservationId in reservationIds)
        {
            var releaseResult = await _releaseSlotService.ReleaseReservationSlotsAsync(reservationId, cancellationToken);
            if (releaseResult.IsSuccess)
            {
                totalReleasedSlots += releaseResult.Value;
                _logger.LogDebug("Released {SlotCount} slots for reservation {ReservationId}", 
                    releaseResult.Value, reservationId.Value);
            }
            else
            {
                var errorMessage = $"Failed to release slots for reservation {reservationId.Value}: {string.Join(", ", releaseResult.Errors.Select(e => e.Message))}";
                releaseErrors.Add(errorMessage);
                _logger.LogWarning(errorMessage);
            }
        }

        // Si il y a eu des erreurs lors de la libération, on log mais on continue quand même
        if (releaseErrors.Any())
        {
            _logger.LogWarning("Some slots could not be released while clearing basket {BasketId}. Errors: {Errors}", 
                request.BasketId, string.Join("; ", releaseErrors));
        }

        var cancelledReservations = 0;
        var cancelErrors = new List<string>();

        _logger.LogInformation("Cancelling {ReservationCount} reservations in basket {BasketId}", 
            basket.Items.Count, request.BasketId);

        foreach (var basketItem in basket.Items)
        {
            var reservation = basketItem.Reservation;
            if (reservation is null)
            {
                var errorMessage = $"Reservation {basketItem.ReservationId.Value} not loaded with basket item";
                cancelErrors.Add(errorMessage);
                _logger.LogWarning(errorMessage);
                continue;
            }

            try
            {
                reservation.Cancel();
                
                // Sauvegarder la réservation annulée
                var updatedReservation = await _reservationRepository.UpdateAsync(reservation, cancellationToken);
                if (updatedReservation is null)
                {
                    var errorMessage = $"Failed to update cancelled reservation {reservation.Id.Value}";
                    cancelErrors.Add(errorMessage);
                    _logger.LogWarning(errorMessage);
                    continue;
                }
                
                cancelledReservations++;
                _logger.LogDebug("Cancelled reservation {ReservationId}", reservation.Id.Value);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to cancel reservation {reservation.Id.Value}: {ex.Message}";
                cancelErrors.Add(errorMessage);
                _logger.LogWarning(ex, errorMessage);
            }
        }

        // Log des erreurs d'annulation si nécessaire
        if (cancelErrors.Any())
        {
            _logger.LogWarning("Some reservations could not be cancelled while clearing basket {BasketId}. Errors: {Errors}", 
                request.BasketId, string.Join("; ", cancelErrors));
        }

        // On vide les éléments du panier uniquement si il est crée, autrement on le laisse en l'état après avoir libéré les créneaux réservés et annulé les réservations.
        if(basket.Status == BasketStatus.Created)
        {
            var clearResult = basket.ClearItems();
            if (clearResult.IsFailed)
                return clearResult;
        }        

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        // Sauvegarder toutes les modifications dans une transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully cleared basket {BasketId}. Released {SlotCount} slots total and cancelled {ReservationCount} reservations.", 
            request.BasketId, totalReleasedSlots, cancelledReservations);

        return Result.Ok();
    }
}