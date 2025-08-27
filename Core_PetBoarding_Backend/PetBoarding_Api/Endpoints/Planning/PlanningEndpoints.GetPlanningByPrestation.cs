namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Api.Mappers.Planning;
using PetBoarding_Application.Planning.GetPlanningByPrestationId;

public static partial class PlanningEndpoints
{
    public static async Task&lt;IResult&gt; GetPlanningByPrestation(string prestationId, ISender sender)
    {
        var query = new GetPlanningByPrestationIdQuery(prestationId);
        var planning = await sender.Send(query);

        if (planning == null)
        {
            return Results.NotFound(new GetPlanningResponse
            {
                Success = false,
                Message = "Planning non trouvé pour cette prestation",
                Data = null
            });
        }

        var response = new GetPlanningResponse
        {
            Success = true,
            Data = PlanningMapper.ToDto(planning),
            Message = "Planning trouvé"
        };

        return Results.Ok(response);
    }
}