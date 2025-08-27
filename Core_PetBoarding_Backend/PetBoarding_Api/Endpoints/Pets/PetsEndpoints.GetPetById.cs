using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Pets.GetPetById;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> GetPetById(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPetByIdQuery(new PetId(id));

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            PetMapper.ToDto,
            PetResponseMapper.ToGetPetResponse);
    }
}