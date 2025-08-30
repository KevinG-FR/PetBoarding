namespace PetBoarding_Application.Baskets.GetUserBasket;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;

public sealed record GetUserBasketQuery(Guid UserId) : IQuery<Basket?>;