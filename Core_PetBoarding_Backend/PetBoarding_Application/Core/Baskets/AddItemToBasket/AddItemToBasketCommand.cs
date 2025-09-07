namespace PetBoarding_Application.Core.Baskets.AddItemToBasket;

using PetBoarding_Application.Core.Abstractions;

public sealed record AddItemToBasketCommand(
    Guid UserId,
    Guid ReservationId
) : ICommand;