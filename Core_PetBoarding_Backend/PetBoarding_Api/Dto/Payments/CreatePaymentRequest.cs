namespace PetBoarding_Api.Dto.Payments;

using System.ComponentModel.DataAnnotations;

public sealed record CreatePaymentRequest(
    Guid BasketId,
    string Method,
    string? Description = null
);