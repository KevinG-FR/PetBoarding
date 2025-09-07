namespace PetBoarding_Application.Core.Payments.CreatePayment;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Payments;

public sealed record CreatePaymentCommand(
    Guid UserId,
    Guid BasketId,
    string Method,
    string? Description
) : ICommand<Payment>;