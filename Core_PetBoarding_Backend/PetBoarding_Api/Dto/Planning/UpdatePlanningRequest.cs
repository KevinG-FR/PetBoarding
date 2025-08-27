using System.ComponentModel.DataAnnotations;

namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// Requête de mise à jour d'un planning
/// </summary>
public sealed record UpdatePlanningRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Nom { get; init; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; init; }
    
    public bool? EstActif { get; init; }
}