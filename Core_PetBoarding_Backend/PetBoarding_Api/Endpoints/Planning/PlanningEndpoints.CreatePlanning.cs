namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Application.Planning.CreatePlanning;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> CreatePlanning(CreatePlanningRequest request, ISender sender)
    {
        try
        {
            var command = new CreatePlanningCommand(request.PrestationId, request.Nom, request.Description);
            var result = await sender.Send(command);

            var response = new CreatePlanningResponse
            {
                Success = true,
                PlanningId = result.Value,
                Message = "Planning créé avec succès"
            };

            return Results.Created($"/api/v1/planning/{result.Value}", response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new CreatePlanningResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new CreatePlanningResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}