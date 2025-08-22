namespace PetBoarding_Api.Endpoints.Prestations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Prestations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Prestations.GetPrestationById;

public static partial class PrestationsEndpoints
{
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
}
