using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.DeletePet;

public sealed record DeletePetCommand(PetId PetId) : ICommand<bool>;