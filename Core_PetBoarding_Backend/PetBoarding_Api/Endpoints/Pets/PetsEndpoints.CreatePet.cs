using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Pets.CreatePet;
using PetBoarding_Domain.Users;
using System.Security.Claims;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> CreatePet(
        [FromBody] CreatePetRequest request,
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

        var command = new CreatePetCommand(
            Name: request.Name,
            Type: request.Type,
            Breed: request.Breed,
            Age: request.Age,
            Color: request.Color,
            Gender: request.Gender,
            IsNeutered: request.IsNeutered,
            OwnerId: new UserId(userId),
            Weight: request.Weight,
            MicrochipNumber: request.MicrochipNumber,
            MedicalNotes: request.MedicalNotes,
            SpecialNeeds: request.SpecialNeeds,
            PhotoUrl: request.PhotoUrl,
            EmergencyContactName: request.EmergencyContactName,
            EmergencyContactPhone: request.EmergencyContactPhone,
            EmergencyContactRelationship: request.EmergencyContactRelationship);

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            PetMapper.ToDto,
            PetResponseMapper.ToCreatePetResponse,
            pet => $"{RouteBase}/{pet.Id.Value}",
            SuccessStatusCode.Created);
    }
}