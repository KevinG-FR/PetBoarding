namespace PetBoarding_Api.Dto.Reservations;

public sealed record ReservationResponse
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required string AnimalId { get; init; }
    public required string AnimalName { get; init; }
    public required string ServiceId { get; init; }
    public required DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Comments { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public decimal? TotalPrice { get; init; }
    public required int NumberOfDays { get; init; }
    public required IEnumerable<DateTime> ReservedDates { get; init; }
}
