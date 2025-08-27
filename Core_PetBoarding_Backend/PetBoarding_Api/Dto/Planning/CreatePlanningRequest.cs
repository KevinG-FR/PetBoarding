using System.ComponentModel.DataAnnotations;

namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// Requête de création d'un planning
/// </summary>
public sealed record CreatePlanningRequest
{
    [Required]
    public string PrestationId { get; init; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Nom { get; init; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; init; }
}