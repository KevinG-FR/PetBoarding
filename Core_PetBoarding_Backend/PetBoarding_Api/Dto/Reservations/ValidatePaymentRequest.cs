namespace PetBoarding_Api.Dto.Reservations;

/// <summary>
/// Request DTO pour valider le paiement d'une réservation
/// </summary>
public sealed class ValidatePaymentRequest
{
    public decimal AmountPaid { get; init; }
    public string? PaymentMethod { get; init; }
}