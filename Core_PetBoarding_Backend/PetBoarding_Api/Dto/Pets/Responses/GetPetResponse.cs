namespace PetBoarding_Api.Dto.Pets.Responses;

public record GetPetResponse
{
    public PetDto Pet { get; init; } = null!;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}