namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Application.Planning.ReserverCreneaux;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> ReserverCreneaux(ReserverCreneauxRequest request, ISender sender)
    {
        var command = new ReserverCreneauxCommand(
            request.PrestationId,
            request.DateDebut,
            request.DateFin,
            request.Quantite);

        var success = await sender.Send(command);

        var response = new ReservationResponse
        {
            Success = success,
            Message = success ? "Créneaux réservés avec succès" : "Échec de la réservation"
        };

        return success ? Results.Ok(response) : Results.BadRequest(response);
    }
}