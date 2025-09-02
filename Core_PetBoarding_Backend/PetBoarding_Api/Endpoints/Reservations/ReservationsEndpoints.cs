namespace PetBoarding_Api.Endpoints.Reservations;

using PetBoarding_Api.Dto.Reservations.Responses;

public static partial class ReservationsEndpoints
{
    private const string RouteBase = "/api/v1/reservations";

    public static void MapReservationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
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
            .Produces<CreateReservationResponse>(201)
            .Produces(400);

        group.MapPut("/{id}", UpdateReservation)
            .WithName("UpdateReservation")
            .WithSummary("Update an existing reservation")
            .WithDescription("Allows updating dates or comments of a reservation.")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapPut("/{id}/cancel", CancelReservation)
            .WithName("CancelReservation")
            .WithSummary("Cancel a reservation")
            .WithDescription("Changes the status of a reservation to 'Cancelled'.")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id}/validate-payment", ValidatePayment)
            .WithName("ValidatePayment")
            .WithSummary("Validate payment for a reservation")
            .WithDescription("Confirms payment and changes reservation status from Created to Validated.")
            .Produces(200)
            .Produces(400)
            .Produces(404);

        group.MapGet("/user/{userId}/displayed", GetUserDisplayedReservations)
            .WithName("GetUserDisplayedReservations")
            .WithSummary("Get displayed reservations for a specific user")
            .WithDescription("Returns only the reservations with status Validated, InProgress, or Completed for a given user ID.")
            .Produces<GetAllReservationsResponse>()
            .Produces(200);
    }
}
