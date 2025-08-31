namespace PetBoarding_Application.Baskets.RemoveItemFromBasket;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Planning.ReleaseSlots;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;
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
        var reservationId = new ReservationId(request.ReservationId);

        _logger.LogInformation("Removing reservation {ReservationId} from basket for user {UserId}", 
            request.ReservationId, request.UserId);

        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket is null)
            return Result.Fail("Basket not found");

        // Libérer les créneaux avant de supprimer la réservation du panier
        var releaseResult = await _releaseSlotService.ReleaseReservationSlotsAsync(reservationId, cancellationToken);
        if (releaseResult.IsFailed)
        {
            _logger.LogError("Failed to release slots for reservation {ReservationId}: {Errors}", 
                request.ReservationId, string.Join(", ", releaseResult.Errors.Select(e => e.Message)));
            return releaseResult.ToResult();
        }

        _logger.LogInformation("Released {SlotCount} slots for reservation {ReservationId}", 
            releaseResult.Value, request.ReservationId);

        var removeReservationResult = basket.RemoveReservation(reservationId);
        if (removeReservationResult.IsFailed)
            return removeReservationResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        _logger.LogInformation("Successfully removed reservation {ReservationId} from basket for user {UserId}. Released {SlotCount} slots.", 
            request.ReservationId, request.UserId, releaseResult.Value);

        return Result.Ok();
    }
}