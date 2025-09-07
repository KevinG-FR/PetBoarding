namespace PetBoarding_Application.Core.Email.Models;

public sealed class ReservationConfirmationModel
{
    public string CustomerName { get; init; } = string.Empty;
    public string PetName { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string ReservationNumber { get; init; } = string.Empty;
    public List<string> SpecialInstructions { get; init; } = new();
    public int Duration => (EndDate - StartDate).Days;
}