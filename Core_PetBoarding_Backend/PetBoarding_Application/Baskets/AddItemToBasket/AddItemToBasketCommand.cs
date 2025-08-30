namespace PetBoarding_Application.Baskets.AddItemToBasket;

using PetBoarding_Application.Abstractions;

public sealed record AddItemToBasketCommand(
    Guid UserId,
    Guid ReservationId
) : ICommand;