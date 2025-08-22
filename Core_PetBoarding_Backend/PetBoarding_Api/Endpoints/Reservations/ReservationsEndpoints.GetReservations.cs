namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Reservations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Reservations.GetReservations;
using PetBoarding_Domain.Reservations;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> GetReservations(
        [FromServices] IMediator mediator,
        [FromQuery] string? userId,
        [FromQuery] string? serviceId,
        [FromQuery] ReservationStatus? status,
        [FromQuery] DateTime? startDateMin,
        [FromQuery] DateTime? startDateMax,
        CancellationToken cancellationToken)
    {
        var query = new GetReservationsQuery(
            userId,
            serviceId,
            status,
            startDateMin,
            startDateMax);

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            ReservationMapper.ToDto,
            ReservationResponseMapper.ToGetAllReservationsResponse);
    }
}
