namespace PetBoarding_Application.Baskets.RemoveItemFromBasket;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Planning.ReleaseSlots;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Users;

internal sealed class RemoveItemFromBasketCommandHandler : ICommandHandler<RemoveItemFromBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IReleaseSlotService _releaseSlotService;
    private readonly ILogger<RemoveItemFromBasketCommandHandler> _logger;

    public RemoveItemFromBasketCommandHandler(
        IBasketRepository basketRepository,
        IReleaseSlotService releaseSlotService,
        ILogger<RemoveItemFromBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _releaseSlotService = releaseSlotService;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var basketItemId = new BasketItemId(request.BasketItemId);

        _logger.LogInformation("Removing basket item {BasketItemId} from basket for user {UserId}", 
            request.BasketItemId, request.UserId);

        var basket = await _basketRepository.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (basket is null)
            return Result.Fail("Basket not found");
            
        var basketItem = basket.Items.FirstOrDefault(item => item.Id == basketItemId);
        if (basketItem is null)
        {
            _logger.LogWarning("Basket item {BasketItemId} not found in basket {BasketId} for user {UserId}. Available items: [{AvailableItems}]", 
                basketItemId.Value, basket.Id.Value, userId.Value, 
                string.Join(", ", basket.Items.Select(i => $"ID:{i.Id.Value}-Res:{i.ReservationId.Value}")));
            return Result.Fail("Basket item not found");
        }

        var reservationId = basketItem.ReservationId;

        // Libérer les créneaux avant de supprimer la réservation du panier
        var releaseResult = await _releaseSlotService.ReleaseReservationSlotsAsync(reservationId, cancellationToken);
        if (releaseResult.IsFailed)
        {
            _logger.LogError("Failed to release slots for reservation {ReservationId}: {Errors}", 
                reservationId.Value, string.Join(", ", releaseResult.Errors.Select(e => e.Message)));
            return releaseResult.ToResult();
        }

        _logger.LogInformation("Released {SlotCount} slots for reservation {ReservationId}", 
            releaseResult.Value, reservationId.Value);

        // Annuler la réservation associée à l'item du panier
        var reservation = basketItem.Reservation;
        if (reservation is null)
        {
            _logger.LogWarning("Reservation {ReservationId} not loaded with basket item", reservationId.Value);
            return Result.Fail("Reservation not found");
        }

        try
        {
            reservation.Cancel();
            _logger.LogInformation("Cancelled reservation {ReservationId}", reservationId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel reservation {ReservationId}: {Message}", reservationId.Value, ex.Message);
            return Result.Fail($"Failed to cancel reservation {reservationId.Value}: {ex.Message}");
        }

        var removeReservationResult = basket.RemoveBasketItem(basketItem.Id);
        if (removeReservationResult.IsFailed)
            return removeReservationResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        _logger.LogInformation("Successfully removed basket item {BasketItemId} (reservation {ReservationId}) from basket for user {UserId}. Released {SlotCount} slots and cancelled reservation.", 
            request.BasketItemId, reservationId.Value, request.UserId, releaseResult.Value);

        return Result.Ok();
    }
}