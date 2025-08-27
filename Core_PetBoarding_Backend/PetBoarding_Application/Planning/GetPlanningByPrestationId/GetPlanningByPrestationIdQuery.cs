namespace PetBoarding_Application.Planning.GetPlanningByPrestationId;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Planning;

public sealed record GetPlanningByPrestationIdQuery(string PrestationId) : IQuery<Planning>;