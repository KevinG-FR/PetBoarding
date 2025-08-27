using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.DeletePet;

public sealed record DeletePetCommand(PetId PetId) : ICommand;