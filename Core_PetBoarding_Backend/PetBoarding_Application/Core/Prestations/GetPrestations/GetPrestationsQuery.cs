namespace PetBoarding_Application.Core.Prestations.GetPrestations;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record GetPrestationsQuery(
    TypeAnimal? CategorieAnimal = null,
    bool? EstDisponible = null,
    string? SearchText = null) : IQuery<IEnumerable<Prestation>>;
