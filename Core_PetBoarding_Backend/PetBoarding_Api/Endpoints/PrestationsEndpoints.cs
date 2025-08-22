namespace PetBoarding_Api.Endpoints;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using PetBoarding_Api.Dto.Prestations;
using PetBoarding_Api.Dto.Prestations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Prestations.CreatePrestation;
using PetBoarding_Application.Prestations.DeletePrestation;
using PetBoarding_Application.Prestations.GetPrestationById;
using PetBoarding_Application.Prestations.GetPrestations;
using PetBoarding_Application.Prestations.UpdatePrestation;
using PetBoarding_Domain.Prestations;

public static class PrestationsEndpoints
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

    private static async Task<IResult> GetPrestations(
        [FromServices] IMediator mediator,
        [FromQuery] TypeAnimal? categorieAnimal,
        [FromQuery] bool? estDisponible,
        [FromQuery] string? searchText,
        CancellationToken cancellationToken)
    {
        var query = new GetPrestationsQuery(
            categorieAnimal,
            estDisponible,
            searchText);

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            PrestationMapper.ToDto,
            PrestationResponseMapper.ToGetAllPrestationsResponse);
    }

    private static async Task<IResult> GetPrestationById(
        [FromRoute] string id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPrestationByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            PrestationMapper.ToDto,
            PrestationResponseMapper.ToGetPrestationResponse);
    }

    private static async Task<IResult> CreatePrestation(
        [FromBody] CreatePrestationRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreatePrestationCommand(
            request.Libelle,
            request.Description,
            request.CategorieAnimal,
            request.Prix,
            request.DureeEnMinutes);

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            PrestationMapper.ToDto,
            PrestationResponseMapper.ToCreatePrestationResponse,
            prestation => $"{RouteBase}/{prestation.Id.Value}",
            SuccessStatusCode.Created);
    }

    private static async Task<IResult> UpdatePrestation(
        [FromRoute] string id,
        [FromBody] UpdatePrestationRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePrestationCommand(
            id,
            request.Libelle,
            request.Description,
            request.CategorieAnimal,
            request.Prix,
            request.DureeEnMinutes,
            request.EstDisponible);

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            PrestationMapper.ToDto,
            PrestationResponseMapper.ToUpdatePrestationResponse);
    }

    private static async Task<IResult> DeletePrestation(
        [FromRoute] string id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeletePrestationCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(PrestationResponseMapper.ToDeletePrestationResponse);
    }
}
