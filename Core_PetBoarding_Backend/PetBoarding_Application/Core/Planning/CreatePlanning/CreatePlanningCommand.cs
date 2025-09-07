namespace PetBoarding_Application.Core.Planning.CreatePlanning;

using PetBoarding_Application.Core.Abstractions;

public sealed record CreatePlanningCommand(
    string PrestationId,
    string Nom,
    string? Description) : ICommand<string>;