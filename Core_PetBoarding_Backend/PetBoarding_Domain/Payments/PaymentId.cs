namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Abstractions;

public class PaymentId(Guid Value) : EntityIdentifier(Value);