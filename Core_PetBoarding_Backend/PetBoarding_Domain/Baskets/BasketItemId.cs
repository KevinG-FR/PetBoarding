namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;

public class BasketItemId(Guid Value) : EntityIdentifier(Value);