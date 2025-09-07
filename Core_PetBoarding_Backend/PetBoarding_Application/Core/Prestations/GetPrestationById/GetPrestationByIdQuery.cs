namespace PetBoarding_Application.Core.Prestations.GetPrestationById;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record GetPrestationByIdQuery(string Id) : IQuery<Prestation>;
