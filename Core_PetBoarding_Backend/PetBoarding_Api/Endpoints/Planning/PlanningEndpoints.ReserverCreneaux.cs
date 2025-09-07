namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Application.Core.Planning.ReserverCreneaux;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> ReserverCreneaux(ReserverCreneauxRequest request, ISender sender)
    {
        var command = new ReserverCreneauxCommand(
            request.PrestationId,
            request.DateDebut,
            request.DateFin,
            request.Quantite);

        var result = await sender.Send(command);

        var response = new ReservationResponse
        {
            Success = result.Value,
            Message = result.Value ? "Créneaux réservés avec succès" : "Échec de la réservation"
        };

        return result.Value ? Results.Ok(response) : Results.BadRequest(response);
    }
}