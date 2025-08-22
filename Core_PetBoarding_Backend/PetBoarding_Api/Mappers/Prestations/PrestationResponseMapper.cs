namespace PetBoarding_Api.Mappers.Prestations;

using PetBoarding_Api.Dto.Prestations;
using PetBoarding_Api.Dto.Prestations.Responses;

public static class PrestationResponseMapper
{
    public static GetAllPrestationsResponse ToGetAllPrestationsResponse(IReadOnlyList<PrestationDto> prestations)
    {
        return new GetAllPrestationsResponse
        {
            Prestations = prestations,
            TotalCount = prestations.Count
        };
    }

    public static GetPrestationResponse ToGetPrestationResponse(PrestationDto prestation)
    {
        return new GetPrestationResponse
        {
            Prestation = prestation
        };
    }

    public static CreatePrestationResponse ToCreatePrestationResponse(PrestationDto prestation)
    {
        return new CreatePrestationResponse
        {
            Prestation = prestation
        };
    }

    public static UpdatePrestationResponse ToUpdatePrestationResponse(PrestationDto prestation)
    {
        return new UpdatePrestationResponse
        {
            Prestation = prestation
        };
    }

    public static DeletePrestationResponse ToDeletePrestationResponse(bool success)
    {
        return new DeletePrestationResponse
        {
            Success = success,
            Message = success ? "Prestation deleted successfully" : "Failed to delete prestation"
        };
    }
}
