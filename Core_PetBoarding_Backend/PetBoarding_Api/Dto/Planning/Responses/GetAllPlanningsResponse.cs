namespace PetBoarding_Api.Dto.Planning.Responses;

/// <summary>
/// Réponse pour obtenir tous les plannings
/// </summary>
public sealed record GetAllPlanningsResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public List<PlanningDto> Data { get; init; } = new();
}