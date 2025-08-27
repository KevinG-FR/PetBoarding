namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Api.Mappers.Planning;
using PetBoarding_Application.Planning.GetAllPlannings;

public static partial class PlanningEndpoints
{
    public static async Task&lt;IResult&gt; GetAllPlannings(ISender sender)
    {
        var query = new GetAllPlanningsQuery();
        var plannings = await sender.Send(query);

        var response = new GetAllPlanningsResponse
        {
            Success = true,
            Data = plannings.Select(PlanningMapper.ToDto).ToList(),
            Message = $"{plannings.Count} planning(s) trouvé(s)"
        };

        return Results.Ok(response);
    }
}