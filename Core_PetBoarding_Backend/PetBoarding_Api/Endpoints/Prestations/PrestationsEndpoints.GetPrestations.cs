namespace PetBoarding_Api.Endpoints.Prestations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Prestations.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Prestations;
using PetBoarding_Application.Prestations.GetPrestations;
using PetBoarding_Domain.Prestations;

public static partial class PrestationsEndpoints
{
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
}
