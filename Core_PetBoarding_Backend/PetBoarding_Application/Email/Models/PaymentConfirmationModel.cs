namespace PetBoarding_Application.Email.Models;

public sealed class PaymentConfirmationModel
{
    public string CustomerName { get; init; } = string.Empty;
    public string PaymentId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime PaymentDate { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ReservationNumber { get; init; }
    public string? ServiceName { get; init; }
}