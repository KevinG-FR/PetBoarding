namespace PetBoarding_Application.Baskets.RemoveItemFromBasket;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

internal sealed class RemoveItemFromBasketCommandHandler : ICommandHandler<RemoveItemFromBasketCommand>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var reservationId = new ReservationId(request.ReservationId);

        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket == null)
            return Result.Fail("Basket not found");

        var removeReservationResult = basket.RemoveReservation(reservationId);
        if (removeReservationResult.IsFailed)
            return removeReservationResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        return Result.Ok();
    }
}