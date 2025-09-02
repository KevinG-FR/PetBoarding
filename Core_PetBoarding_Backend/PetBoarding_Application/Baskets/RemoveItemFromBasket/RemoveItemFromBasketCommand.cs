namespace PetBoarding_Application.Baskets.RemoveItemFromBasket;

using PetBoarding_Application.Abstractions;

public sealed record RemoveItemFromBasketCommand(
    Guid UserId,
    Guid BasketItemId
) : ICommand;