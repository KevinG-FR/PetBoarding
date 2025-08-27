using System.ComponentModel.DataAnnotations;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Dto.Pets;

public record CreatePetRequest
{
    [Required(ErrorMessage = "Le nom de l'animal est requis")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Le type d'animal est requis")]
    public PetType Type { get; init; }

    [Required(ErrorMessage = "La race est requise")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "La race doit contenir entre 2 et 100 caractères")]
    public string Breed { get; init; } = string.Empty;

    [Required(ErrorMessage = "L'âge est requis")]
    [Range(0, 50, ErrorMessage = "L'âge doit être entre 0 et 50 ans")]
    public int Age { get; init; }

    [Range(0.1, 1000, ErrorMessage = "Le poids doit être entre 0.1 et 1000 kg")]
    public decimal? Weight { get; init; }

    [Required(ErrorMessage = "La couleur est requise")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La couleur doit contenir entre 2 et 50 caractères")]
    public string Color { get; init; } = string.Empty;

    [Required(ErrorMessage = "Le genre est requis")]
    public PetGender Gender { get; init; }

    public bool IsNeutered { get; init; }

    [StringLength(50, ErrorMessage = "Le numéro de puce ne peut pas dépasser 50 caractères")]
    public string? MicrochipNumber { get; init; }

    [StringLength(2000, ErrorMessage = "Les notes médicales ne peuvent pas dépasser 2000 caractères")]
    public string? MedicalNotes { get; init; }

    [StringLength(2000, ErrorMessage = "Les besoins spéciaux ne peuvent pas dépasser 2000 caractères")]
    public string? SpecialNeeds { get; init; }

    [StringLength(500, ErrorMessage = "L'URL de la photo ne peut pas dépasser 500 caractères")]
    public string? PhotoUrl { get; init; }

    [StringLength(100, ErrorMessage = "Le nom du contact d'urgence ne peut pas dépasser 100 caractères")]
    public string? EmergencyContactName { get; init; }

    [StringLength(20, ErrorMessage = "Le téléphone du contact d'urgence ne peut pas dépasser 20 caractères")]
    public string? EmergencyContactPhone { get; init; }

    [StringLength(50, ErrorMessage = "La relation du contact d'urgence ne peut pas dépasser 50 caractères")]
    public string? EmergencyContactRelationship { get; init; }
}