namespace PetBoarding_Application.Baskets.UpdateBasketItem;

using PetBoarding_Application.Abstractions;

public sealed record UpdateBasketItemCommand(
    Guid UserId,
    Guid PrestationId,
    int NewQuantity
) : ICommand;