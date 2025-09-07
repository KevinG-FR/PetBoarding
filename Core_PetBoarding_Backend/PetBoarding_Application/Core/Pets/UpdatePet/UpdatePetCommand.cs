using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.UpdatePet;

public sealed record UpdatePetCommand(
    PetId PetId,
    string Name,
    PetType Type,
    string Breed,
    int Age,
    string Color,
    PetGender Gender,
    bool IsNeutered,
    decimal? Weight = null,
    string? MicrochipNumber = null,
    string? MedicalNotes = null,
    string? SpecialNeeds = null,
    string? PhotoUrl = null,
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null,
    string? EmergencyContactRelationship = null) : ICommand<Pet>;