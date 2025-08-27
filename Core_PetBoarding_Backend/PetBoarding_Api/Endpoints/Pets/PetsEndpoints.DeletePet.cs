using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Pets.DeletePet;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> DeletePet(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetCommand(new PetId(id));

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            () => new { PetId = id },
            PetResponseMapper.ToDeletePetResponse);
    }
}