namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse de vérification de disponibilité
/// </summary>
public sealed record DisponibiliteResponse
{
    public string PrestationId { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public List<CreneauDisponibleDto> AvailableSlots { get; init; } = new();
    public string? Message { get; init; }
}