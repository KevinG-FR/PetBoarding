namespace PetBoarding_Api.Endpoints.Prestations;

using PetBoarding_Api.Dto.Prestations.Responses;

public static partial class PrestationsEndpoints
{
    private const string RouteBase = "/api/v1/prestations";

    public static void MapPrestationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
            .WithTags(Tags.Prestations);

        group.MapGet("/", GetPrestations)
            .WithName("GetPrestations")
            .WithSummary("Get list of prestations with optional filters")
            .WithDescription("Allows retrieving all prestations or filtering them by category, availability, etc.")
            .Produces<GetAllPrestationsResponse>();

        group.MapGet("/{id}", GetPrestationById)
            .WithName("GetPrestationById")
            .WithSummary("Get a prestation by its identifier")
            .WithDescription("Returns complete details of a specific prestation.")
            .Produces<GetPrestationResponse>()
            .Produces(404);

        group.MapPost("/", CreatePrestation)
            .WithName("CreatePrestation")
            .WithSummary("Create a new prestation")
            .WithDescription("Allows creating a new prestation service.")
            .Produces<CreatePrestationResponse>(201)
            .Produces(400);

        group.MapPut("/{id}", UpdatePrestation)
            .WithName("UpdatePrestation")
            .WithSummary("Update an existing prestation")
            .WithDescription("Allows updating properties of a prestation.")
            .Produces<UpdatePrestationResponse>(200)
            .Produces(400)
            .Produces(404);

        group.MapDelete("/{id}", DeletePrestation)
            .WithName("DeletePrestation")
            .WithSummary("Delete a prestation")
            .WithDescription("Removes a prestation from the system.")
            .Produces<DeletePrestationResponse>(200)
            .Produces(400)
            .Produces(404);
    }
}
