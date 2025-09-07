namespace PetBoarding_Application.Core.Planning.GetPlanningByPrestationId;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Planning;

public sealed record GetPlanningByPrestationIdQuery(string PrestationId) : IQuery<Planning>;