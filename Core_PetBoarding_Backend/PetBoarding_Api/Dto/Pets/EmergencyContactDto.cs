namespace PetBoarding_Api.Dto.Pets;

public record EmergencyContactDto
{
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Relationship { get; init; } = string.Empty;
}