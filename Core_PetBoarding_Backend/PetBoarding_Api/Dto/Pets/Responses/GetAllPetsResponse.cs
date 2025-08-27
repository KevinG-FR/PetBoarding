namespace PetBoarding_Api.Dto.Pets.Responses;

public record GetAllPetsResponse
{
    public IEnumerable<PetDto> Pets { get; init; } = Enumerable.Empty<PetDto>();
    public int Count { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}