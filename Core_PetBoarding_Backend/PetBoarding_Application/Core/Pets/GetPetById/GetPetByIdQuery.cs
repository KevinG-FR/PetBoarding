using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.GetPetById;

public sealed record GetPetByIdQuery(PetId PetId) : IQuery<Pet>;