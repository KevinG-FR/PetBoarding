namespace PetBoarding_Application.Planning.GetAllPlannings;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Planning;

public sealed record GetAllPlanningsQuery : IQuery<List<Planning>>;