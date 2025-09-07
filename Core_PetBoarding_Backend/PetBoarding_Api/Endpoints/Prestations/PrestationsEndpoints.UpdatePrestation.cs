namespace PetBoarding_Api.Endpoints.Prestations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Prestations;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Core.Prestations.UpdatePrestation;

public static partial class PrestationsEndpoints
{
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
}
