namespace PetBoarding_Application.Prestations.UpdatePrestation;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record UpdatePrestationCommand(
    string Id,
    string? Libelle = null,
    string? Description = null,
    TypeAnimal? CategorieAnimal = null,
    decimal? Prix = null,
    int? DureeEnMinutes = null,
    bool? EstDisponible = null) : ICommand<Prestation>;
