namespace PetBoarding_Application.Baskets.AddItemToBasket;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

internal sealed class AddItemToBasketCommandHandler : ICommandHandler<AddItemToBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IReservationRepository _reservationRepository;

    public AddItemToBasketCommandHandler(
        IBasketRepository basketRepository,
        IReservationRepository reservationRepository)
    {
        _basketRepository = basketRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var reservationId = new ReservationId(request.ReservationId);

        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation == null)
            return Result.Fail("Reservation not found");

        if (reservation.UserId != request.UserId.ToString())
            return Result.Fail("Reservation does not belong to the user");

        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        
        if (basket == null)
        {
            basket = new Basket(userId);
            await _basketRepository.AddAsync(basket, cancellationToken);
        }

        var addReservationResult = basket.AddReservation(reservationId);
        if (addReservationResult.IsFailed)
            return addReservationResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        return Result.Ok();
    }
}