namespace PetBoarding_Api.Endpoints.Planning;

using PetBoarding_Api.Dto.Planning.Responses;

public static partial class PlanningEndpoints
{
    private const string RouteBase = "/api/v1/planning";

    public static void MapPlanningEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
            .WithTags(Tags.Planning);

        group.MapGet("/", GetAllPlannings)
            .WithName("GetAllPlannings")
            .WithSummary("Get all active plannings")
            .WithDescription("Retrieve all active planning schedules")
            .Produces<GetAllPlanningsResponse>();

        group.MapGet("/prestation/{prestationId}", GetPlanningByPrestation)
            .WithName("GetPlanningByPrestation")
            .WithSummary("Get planning by prestation ID")
            .WithDescription("Get planning schedule for a specific prestation")
            .Produces<GetPlanningResponse>()
            .Produces(404);

        group.MapPost("/", CreatePlanning)
            .WithName("CreatePlanning")
            .WithSummary("Create a new planning")
            .WithDescription("Create a new planning schedule for a prestation")
            .Produces<CreatePlanningResponse>(201)
            .Produces(400);

        group.MapPost("/disponibilite", VerifierDisponibilite)
            .WithName("VerifierDisponibilite")
            .WithSummary("Check availability")
            .WithDescription("Check availability for dates and capacity")
            .Produces<DisponibiliteResponse>()
            .Produces(400);

        group.MapPost("/reserver", ReserverCreneaux)
            .WithName("ReserverCreneaux")
            .WithSummary("Reserve time slots")
            .WithDescription("Reserve capacity for specified dates")
            .Produces<ReservationResponse>()
            .Produces(400);

        group.MapPost("/annuler", AnnulerReservations)
            .WithName("AnnulerReservations")
            .WithSummary("Cancel reservations")
            .WithDescription("Cancel existing reservations")
            .Produces<ReservationResponse>()
            .Produces(400);
    }
}