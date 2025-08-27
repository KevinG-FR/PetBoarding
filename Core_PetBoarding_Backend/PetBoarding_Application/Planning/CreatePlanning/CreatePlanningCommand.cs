namespace PetBoarding_Application.Planning.CreatePlanning;

using PetBoarding_Application.Abstractions;

public sealed record CreatePlanningCommand(
    string PrestationId,
    string Nom,
    string? Description) : ICommand<string>;