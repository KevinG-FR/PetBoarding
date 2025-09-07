namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Application.Core.Planning.AnnulerReservations;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> AnnulerReservations(ReserverCreneauxRequest request, ISender sender)
    {
        var command = new AnnulerReservationsCommand(
            request.PrestationId,
            request.DateDebut,
            request.DateFin,
            request.Quantite);

        var result = await sender.Send(command);

        var response = new ReservationResponse
        {
            Success = result.Value,
            Message = result.Value ? "Réservations annulées avec succès" : "Échec de l'annulation"
        };

        return Results.Ok(response);
    }
}