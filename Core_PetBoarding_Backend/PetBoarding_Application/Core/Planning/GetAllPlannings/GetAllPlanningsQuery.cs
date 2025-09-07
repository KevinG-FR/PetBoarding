namespace PetBoarding_Application.Core.Planning.GetAllPlannings;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Planning;

public sealed record GetAllPlanningsQuery : IQuery<List<Planning>>;