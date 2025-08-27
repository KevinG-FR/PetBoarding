namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse pour obtenir un planning spécifique
/// </summary>
public sealed record GetPlanningResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public PlanningDto? Data { get; init; }
}