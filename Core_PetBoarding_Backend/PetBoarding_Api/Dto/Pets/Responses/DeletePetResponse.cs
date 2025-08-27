namespace PetBoarding_Api.Dto.Pets.Responses;

public record DeletePetResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? PetId { get; init; }
}