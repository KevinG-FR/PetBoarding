namespace PetBoarding_Api.Dto.Prestations.Responses;

using PetBoarding_Api.Dto.Prestations;

public record GetAllPrestationsResponse
{
    public IEnumerable<PrestationDto> Prestations { get; init; } = [];
    public int TotalCount { get; init; }
}

public record GetPrestationResponse
{
    public PrestationDto Prestation { get; init; } = null!;
}

public record CreatePrestationResponse
{
    public PrestationDto Prestation { get; init; } = null!;
}

public record UpdatePrestationResponse
{
    public PrestationDto Prestation { get; init; } = null!;
}

public record DeletePrestationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}
