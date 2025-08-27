namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// DTO représentant un planning de prestation
/// </summary>
public sealed record PlanningDto
{
    public string Id { get; init; } = string.Empty;
    public string PrestationId { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool EstActif { get; init; }
    public DateTime DateCreation { get; init; }
    public DateTime? DateModification { get; init; }
    public List<CreneauDisponibleDto> Creneaux { get; init; } = new();
}