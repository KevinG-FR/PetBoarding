namespace PetBoarding_Application.Prestations.CreatePrestation;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record CreatePrestationCommand(
    string Libelle,
    string Description,
    TypeAnimal CategorieAnimal,
    decimal Prix,
    int DureeEnMinutes) : ICommand<Prestation>;
