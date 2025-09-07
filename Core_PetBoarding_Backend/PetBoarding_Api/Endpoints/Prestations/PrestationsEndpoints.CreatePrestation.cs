namespace PetBoarding_Api.Endpoints.Prestations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Prestations;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Core.Prestations.CreatePrestation;

public static partial class PrestationsEndpoints
{
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
}
