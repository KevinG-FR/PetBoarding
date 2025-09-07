namespace PetBoarding_Application.Core.Baskets.UpdateBasketItem;

using PetBoarding_Application.Core.Abstractions;

public sealed record UpdateBasketItemCommand(
    Guid UserId,
    Guid PrestationId,
    int NewQuantity
) : ICommand;