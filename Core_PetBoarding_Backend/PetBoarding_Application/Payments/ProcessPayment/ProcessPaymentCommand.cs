namespace PetBoarding_Application.Payments.ProcessPayment;

using PetBoarding_Application.Abstractions;

public sealed record ProcessPaymentCommand(
    Guid PaymentId,
    Guid UserId,
    bool IsSuccessful,
    string? ExternalTransactionId = null,
    string? FailureReason = null
) : ICommand;