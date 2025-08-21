namespace PetBoarding_Api.Dto.Reservations;

public sealed record UpdateReservationRequest
{
    public required string Id { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Comments { get; init; }
}
