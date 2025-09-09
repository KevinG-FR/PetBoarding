namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;

public class BasketId(Guid Value) : EntityIdentifier(Value);