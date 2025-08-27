using System.ComponentModel.DataAnnotations;

namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// Requête de réservation de créneaux
/// </summary>
public sealed record ReserverCreneauxRequest
{
    [Required]
    public string PrestationId { get; init; } = string.Empty;
    
    [Required]
    public DateTime DateDebut { get; init; }
    
    public DateTime? DateFin { get; init; }
    
    [Range(1, int.MaxValue)]
    public int Quantite { get; init; } = 1;
}