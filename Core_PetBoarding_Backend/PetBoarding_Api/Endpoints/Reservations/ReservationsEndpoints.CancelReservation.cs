namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Extensions;
using PetBoarding_Application.Reservations.CancelReservation;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> CancelReservation(
        [FromRoute] string id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return result.GetHttpResult();
    }
}
