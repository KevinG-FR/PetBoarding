namespace PetBoarding_Application.Core.Payments.ProcessPayment;

using PetBoarding_Application.Core.Abstractions;

public sealed record ProcessPaymentCommand(
    Guid PaymentId,
    Guid UserId,
    bool IsSuccessful,
    string? ExternalTransactionId = null,
    string? FailureReason = null
) : ICommand;