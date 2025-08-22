namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Reservations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Reservations.GetReservationById;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> GetReservationById(
        [FromRoute] string id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            ReservationMapper.ToDto,
            ReservationResponseMapper.ToGetReservationResponse);
    }
}
