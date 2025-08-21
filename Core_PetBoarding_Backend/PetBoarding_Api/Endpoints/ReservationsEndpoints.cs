namespace PetBoarding_Api.Endpoints;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Api.Dto.Reservations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Reservations.CancelReservation;
using PetBoarding_Application.Reservations.CreateReservation;
using PetBoarding_Application.Reservations.UpdateReservation;
using PetBoarding_Application.Reservations.GetReservationById;
using PetBoarding_Application.Reservations.GetReservations;
using PetBoarding_Domain.Reservations;

public static class ReservationsEndpoints
{
    public static void MapReservationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reservations")
            .WithTags(Tags.Reservations);

        group.MapGet("/", GetReservations)
            .WithName("GetReservations")
            .WithSummary("Get list of reservations with optional filters")
            .WithDescription("Allows retrieving all reservations or filtering them by user, service, status, etc.")
            .Produces<GetAllReservationsResponse>();

        group.MapGet("/{id}", GetReservationById)
            .WithName("GetReservationById")
            .WithSummary("Get a reservation by its identifier")
            .WithDescription("Returns complete details of a specific reservation.")
            .Produces<GetReservationResponse>()
            .Produces(404);

        group.MapPost("/", CreateReservation)
            .WithName("CreateReservation")
            .WithSummary("Create a new reservation")
            .WithDescription("Allows creating a new reservation for an animal and a service.")
            .Produces<string>(201)
            .Produces(400);

        group.MapPut("/{id}", UpdateReservation)
            .WithName("UpdateReservation")
            .WithSummary("Update an existing reservation")
            .WithDescription("Allows updating dates or comments of a reservation.")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapDelete("/{id}", CancelReservation)
            .WithName("CancelReservation")
            .WithSummary("Cancel a reservation")
            .WithDescription("Changes the status of a reservation to 'Cancelled'.")
            .Produces(204)
            .Produces(400)
            .Produces(404);
    }

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

        return result.GetHttpResult();
    }

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
