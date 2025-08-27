using PetBoarding_Api.Dto.Pets;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Mappers.Pets;

public static class PetMapper
{
    public static PetDto ToDto(Pet pet)
    {
        return new PetDto
        {
            Id = pet.Id.Value,
            Name = pet.Name,
            Type = pet.Type,
            Breed = pet.Breed,
            Age = pet.Age,
            Weight = pet.Weight,
            Color = pet.Color,
            Gender = pet.Gender,
            IsNeutered = pet.IsNeutered,
            MicrochipNumber = pet.MicrochipNumber,
            MedicalNotes = pet.MedicalNotes,
            SpecialNeeds = pet.SpecialNeeds,
            PhotoUrl = pet.PhotoUrl,
            OwnerId = pet.OwnerId.Value,
            EmergencyContact = pet.EmergencyContact != null ? ToEmergencyContactDto(pet.EmergencyContact) : null,
            CreatedAt = pet.CreatedAt,
            UpdatedAt = pet.UpdatedAt
        };
    }

    private static EmergencyContactDto ToEmergencyContactDto(EmergencyContact emergencyContact)
    {
        return new EmergencyContactDto
        {
            Name = emergencyContact.Name,
            Phone = emergencyContact.Phone,
            Relationship = emergencyContact.Relationship
        };
    }
}