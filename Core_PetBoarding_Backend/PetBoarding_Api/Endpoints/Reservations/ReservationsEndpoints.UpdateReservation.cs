namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Api.Extensions;
using PetBoarding_Application.Reservations.UpdateReservation;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> UpdateReservation(
        [FromRoute] string id,
        [FromBody] UpdateReservationRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReservationCommand(
            id,
            request.StartDate,
            request.EndDate,
            request.Comments);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return result.GetHttpResult();
    }
}
