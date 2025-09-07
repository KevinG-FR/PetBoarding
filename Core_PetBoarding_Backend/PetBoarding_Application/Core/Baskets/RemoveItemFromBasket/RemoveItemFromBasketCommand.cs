namespace PetBoarding_Application.Core.Baskets.RemoveItemFromBasket;

using PetBoarding_Application.Core.Abstractions;

public sealed record RemoveItemFromBasketCommand(
    Guid UserId,
    Guid BasketItemId
) : ICommand;