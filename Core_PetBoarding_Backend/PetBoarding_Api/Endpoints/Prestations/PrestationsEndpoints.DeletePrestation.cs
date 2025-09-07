namespace PetBoarding_Api.Endpoints.Prestations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Core.Prestations.DeletePrestation;

public static partial class PrestationsEndpoints
{
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
