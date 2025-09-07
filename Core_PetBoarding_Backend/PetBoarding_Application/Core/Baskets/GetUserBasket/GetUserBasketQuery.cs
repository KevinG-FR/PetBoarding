namespace PetBoarding_Application.Core.Baskets.GetUserBasket;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Baskets;

public sealed record GetUserBasketQuery(Guid UserId) : IQuery<Basket?>;