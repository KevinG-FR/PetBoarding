namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Core.Reservations.GetUserDisplayedReservations;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> GetUserDisplayedReservations(
        [FromServices] IMediator mediator,
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserDisplayedReservationsQuery(userId);

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            ReservationMapper.ToDto,
            ReservationResponseMapper.ToGetAllReservationsResponse);
    }
}