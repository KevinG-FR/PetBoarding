namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse de création d'un planning
/// </summary>
public sealed record CreatePlanningResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? PlanningId { get; init; }
}