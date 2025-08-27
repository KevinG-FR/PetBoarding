namespace PetBoarding_Api.Dto.Pets.Responses;

public record UpdatePetResponse
{
    public PetDto Pet { get; init; } = null!;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}