namespace PetBoarding_Api.Dto.Reservations;

public sealed record CreateReservationRequest
{
    public required string UserId { get; init; }
    public required string AnimalId { get; init; }
    public required string AnimalName { get; init; }
    public required string ServiceId { get; init; }
    public required DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Comments { get; init; }
}
