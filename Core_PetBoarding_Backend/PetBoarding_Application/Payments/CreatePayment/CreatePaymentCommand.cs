namespace PetBoarding_Application.Payments.CreatePayment;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Payments;

public sealed record CreatePaymentCommand(
    Guid UserId,
    Guid BasketId,
    string Method,
    string? Description
) : ICommand<Payment>;