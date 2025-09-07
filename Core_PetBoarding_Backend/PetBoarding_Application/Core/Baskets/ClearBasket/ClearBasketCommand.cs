namespace PetBoarding_Application.Core.Baskets.ClearBasket;

using PetBoarding_Application.Core.Abstractions;

public sealed record ClearBasketCommand(Guid BasketId) : ICommand;