using PetBoarding_Api.Dto.Pets.Responses;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private const string RouteBase = "/api/v1/pets";

    public static void MapPetsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase)
            .WithTags(Tags.Pets);

        group.MapGet("/", GetPets)
            .WithName("GetPets")
            .WithSummary("Get list of pets with optional filters")
            .WithDescription("Allows retrieving all pets or filtering them by owner, type, etc.")
            .Produces<GetAllPetsResponse>();

        group.MapGet("/{id}", GetPetById)
            .WithName("GetPetById")
            .WithSummary("Get a pet by its identifier")
            .WithDescription("Returns complete details of a specific pet.")
            .Produces<GetPetResponse>()
            .Produces(404);

        group.MapGet("/owner/{ownerId}", GetPetsByOwner)
            .WithName("GetPetsByOwner")
            .WithSummary("Get pets by owner")
            .WithDescription("Returns all pets belonging to a specific owner.")
            .Produces<GetAllPetsResponse>();

        group.MapPost("/", CreatePet)
            .WithName("CreatePet")
            .WithSummary("Create a new pet")
            .WithDescription("Allows creating a new pet for a user.")
            .Produces<CreatePetResponse>(201)
            .Produces(400);

        group.MapPut("/{id}", UpdatePet)
            .WithName("UpdatePet")
            .WithSummary("Update an existing pet")
            .WithDescription("Allows updating properties of a pet.")
            .Produces<UpdatePetResponse>(200)
            .Produces(400)
            .Produces(404);

        group.MapDelete("/{id}", DeletePet)
            .WithName("DeletePet")
            .WithSummary("Delete a pet")
            .WithDescription("Removes a pet from the system.")
            .Produces<DeletePetResponse>(200)
            .Produces(400)
            .Produces(404);
    }
}