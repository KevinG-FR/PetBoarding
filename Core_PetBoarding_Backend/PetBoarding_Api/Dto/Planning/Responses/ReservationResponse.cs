namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse des opérations de réservation
/// </summary>
public sealed record ReservationResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
}