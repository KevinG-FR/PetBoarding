namespace PetBoarding_Application.Prestations.DeletePrestation;

using PetBoarding_Application.Abstractions;

public sealed record DeletePrestationCommand(string Id) : ICommand<bool>;
