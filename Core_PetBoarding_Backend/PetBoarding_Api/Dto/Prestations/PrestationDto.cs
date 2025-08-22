namespace PetBoarding_Api.Dto.Prestations;

using PetBoarding_Domain.Prestations;

public record PrestationDto
{
    public Guid Id { get; init; }
    public string Libelle { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TypeAnimal CategorieAnimal { get; init; }
    public decimal Prix { get; init; }
    public int DureeEnMinutes { get; init; }
    public bool EstDisponible { get; init; }
    public DateTime DateCreation { get; init; }
    public DateTime? DateModification { get; init; }
}
