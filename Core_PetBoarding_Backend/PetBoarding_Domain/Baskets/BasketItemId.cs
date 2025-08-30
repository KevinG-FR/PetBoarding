namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;

public record BasketItemId(Guid Value) : EntityIdentifier(Value);