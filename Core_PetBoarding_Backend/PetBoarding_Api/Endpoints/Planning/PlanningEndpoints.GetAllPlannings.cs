namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Api.Mappers.Planning;
using PetBoarding_Application.Planning.GetAllPlannings;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> GetAllPlannings(ISender sender)
    {
        var query = new GetAllPlanningsQuery();
        var result = await sender.Send(query);

        var response = new GetAllPlanningsResponse
        {
            Success = true,
            Data = result.Value.Select(PlanningMapper.ToDto).ToList(),
            Message = $"{result.Value.Count} planning(s) trouvé(s)"
        };

        return Results.Ok(response);
    }
}