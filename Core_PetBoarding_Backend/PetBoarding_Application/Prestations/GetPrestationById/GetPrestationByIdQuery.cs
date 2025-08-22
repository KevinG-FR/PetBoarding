namespace PetBoarding_Application.Prestations.GetPrestationById;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed record GetPrestationByIdQuery(string Id) : IQuery<Prestation>;
