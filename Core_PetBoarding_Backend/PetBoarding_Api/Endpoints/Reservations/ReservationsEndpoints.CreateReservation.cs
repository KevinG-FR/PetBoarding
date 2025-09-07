namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Core.Reservations.CreateReservation;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> CreateReservation(
        [FromBody] CreateReservationRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            request.UserId,
            request.AnimalId,
            request.AnimalName,
            request.ServiceId,
            request.StartDate,
            request.EndDate,
            request.Comments);

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            ReservationMapper.ToDto,
            ReservationResponseMapper.ToCreateReservationResponse,
            reservation => $"/api/v1/reservations/{reservation.Id.Value}",
            SuccessStatusCode.Created);
    }
}
