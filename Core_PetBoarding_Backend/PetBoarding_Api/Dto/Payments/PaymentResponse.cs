namespace PetBoarding_Api.Dto.Payments;

public sealed record PaymentResponse(
    Guid Id,
    decimal Amount,
    string Method,
    string Status,
    string? ExternalTransactionId,
    string? Description,
    DateTime? ProcessedAt,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime UpdatedAt
);