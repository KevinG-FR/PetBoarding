using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Core.Pets.GetPetsByOwner;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> GetPetsByOwner(
        [FromRoute] Guid ownerId,
        [FromQuery] PetType? type,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPetsByOwnerQuery(new UserId(ownerId), type);

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            pets => pets.Select(PetMapper.ToDto),
            PetResponseMapper.ToGetAllPetsResponse);
    }
}