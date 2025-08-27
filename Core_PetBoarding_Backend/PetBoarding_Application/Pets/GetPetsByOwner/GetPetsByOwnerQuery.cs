using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Pets.GetPetsByOwner;

public sealed record GetPetsByOwnerQuery(
    UserId OwnerId,
    PetType? Type = null) : IQuery<IEnumerable<Pet>>;