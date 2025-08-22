namespace PetBoarding_Application.Prestations.GetPrestations;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record GetPrestationsQuery(
    TypeAnimal? CategorieAnimal = null,
    bool? EstDisponible = null,
    string? SearchText = null) : IQuery<IEnumerable<Prestation>>;
