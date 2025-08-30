namespace PetBoarding_Application.Payments.ProcessPayment;

using PetBoarding_Application.Abstractions;

public sealed record ProcessPaymentCommand(
    Guid PaymentId,
    bool IsSuccessful,
    string? ExternalTransactionId = null,
    string? FailureReason = null
) : ICommand;