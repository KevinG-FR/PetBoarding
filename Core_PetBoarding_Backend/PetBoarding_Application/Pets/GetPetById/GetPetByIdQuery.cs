using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.GetPetById;

public sealed record GetPetByIdQuery(PetId PetId) : IQuery<Pet>;