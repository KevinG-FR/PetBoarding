namespace PetBoarding_Api.Dto.Prestations;

using PetBoarding_Domain.Prestations;

public record UpdatePrestationRequest
{
    public string? Libelle { get; init; }
    public string? Description { get; init; }
    public TypeAnimal? CategorieAnimal { get; init; }
    public decimal? Prix { get; init; }
    public int? DureeEnMinutes { get; init; }
    public bool? EstDisponible { get; init; }
}
