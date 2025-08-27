namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// DTO pour les requêtes de vérification de disponibilité
/// </summary>
public sealed record DisponibiliteQueryDto
{
    public string PrestationId { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int? Quantity { get; init; }
}