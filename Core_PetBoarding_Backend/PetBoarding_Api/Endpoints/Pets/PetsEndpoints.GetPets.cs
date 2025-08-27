using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Pets.GetPetsByOwner;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;
using System.Security.Claims;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> GetPets(
        [FromQuery] PetType? type,
        [FromServices] IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        // Récupérer l'ID de l'utilisateur connecté depuis les claims
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        // Pour le moment, on retourne les pets de l'utilisateur connecté
        // On pourrait ajouter une logique d'autorisation pour les admins plus tard
        var query = new GetPetsByOwnerQuery(new UserId(userId), type);

        var result = await mediator.Send(query, cancellationToken);

        return result.GetHttpResult(
            pets => pets.Select(PetMapper.ToDto),
            PetResponseMapper.ToGetAllPetsResponse);
    }
}