namespace PetBoarding_Api.Dto.Pets.Responses;

public record PetResponse
{
    public PetDto Pet { get; init; } = null!;
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}