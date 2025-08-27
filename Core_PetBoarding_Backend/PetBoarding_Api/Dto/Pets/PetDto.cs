using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Dto.Pets;

public record PetDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public PetType Type { get; init; }
    public string Breed { get; init; } = string.Empty;
    public int Age { get; init; }
    public decimal? Weight { get; init; }
    public string Color { get; init; } = string.Empty;
    public PetGender Gender { get; init; }
    public bool IsNeutered { get; init; }
    public string? MicrochipNumber { get; init; }
    public string? MedicalNotes { get; init; }
    public string? SpecialNeeds { get; init; }
    public string? PhotoUrl { get; init; }
    public Guid OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public EmergencyContactDto? EmergencyContact { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}