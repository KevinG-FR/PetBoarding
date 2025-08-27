namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse de mise à jour d'un planning
/// </summary>
public sealed record UpdatePlanningResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
}