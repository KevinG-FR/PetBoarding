namespace PetBoarding_Application.Baskets.ClearBasket;

using PetBoarding_Application.Abstractions;

public sealed record ClearBasketCommand(Guid BasketId) : ICommand;