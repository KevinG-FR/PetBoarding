using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Pets.CreatePet;

public sealed record CreatePetCommand(
    string Name,
    PetType Type,
    string Breed,
    int Age,
    string Color,
    PetGender Gender,
    bool IsNeutered,
    UserId OwnerId,
    decimal? Weight = null,
    string? MicrochipNumber = null,
    string? MedicalNotes = null,
    string? SpecialNeeds = null,
    string? PhotoUrl = null,
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null,
    string? EmergencyContactRelationship = null) : ICommand<Pet>;