namespace PetBoarding_Application.Core.Prestations.DeletePrestation;

using PetBoarding_Application.Core.Abstractions;

public sealed record DeletePrestationCommand(string Id) : ICommand<bool>;
