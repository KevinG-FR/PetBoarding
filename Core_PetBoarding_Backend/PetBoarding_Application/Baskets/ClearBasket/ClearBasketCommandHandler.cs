namespace PetBoarding_Application.Baskets.ClearBasket;

using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Planning.ReleaseSlots;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Users;

internal sealed class ClearBasketCommandHandler : ICommandHandler<ClearBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IReleaseSlotService _releaseSlotService;
    private readonly ILogger<ClearBasketCommandHandler> _logger;

    public ClearBasketCommandHandler(
        IBasketRepository basketRepository,
        IReleaseSlotService releaseSlotService,
        ILogger<ClearBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _releaseSlotService = releaseSlotService;
        _logger = logger;
    }

    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        _logger.LogInformation("Clearing basket for user {UserId}", request.UserId);

        var basket = await _basketRepository.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (basket is null)
            return Result.Fail("Basket not found");

        // Libérer les créneaux de toutes les réservations du panier avant de le vider
        var reservationIds = basket.GetReservationIds().ToList();
        var totalReleasedSlots = 0;
        var releaseErrors = new List<string>();

        _logger.LogInformation("Releasing slots for {ReservationCount} reservations in basket for user {UserId}", 
            reservationIds.Count, request.UserId);

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
            _logger.LogWarning("Some slots could not be released while clearing basket for user {UserId}. Errors: {Errors}", 
                request.UserId, string.Join("; ", releaseErrors));
        }

        // On vide les éléments du panier uniquement si il est crée, autrement on le laisse en l'état après avoir libberé les créneaux réservés.
        if(basket.Status == BasketStatus.Created)
        {
            var clearResult = basket.ClearItems();
            if (clearResult.IsFailed)
                return clearResult;
        }        

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        _logger.LogInformation("Successfully cleared basket for user {UserId}. Released {SlotCount} slots total.", 
            request.UserId, totalReleasedSlots);

        return Result.Ok();
    }
}