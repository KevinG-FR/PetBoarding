namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;

public record BasketId(Guid Value) : EntityIdentifier(Value);