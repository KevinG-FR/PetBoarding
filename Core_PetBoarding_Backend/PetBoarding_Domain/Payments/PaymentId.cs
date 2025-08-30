namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Abstractions;

public record PaymentId(Guid Value) : EntityIdentifier(Value);