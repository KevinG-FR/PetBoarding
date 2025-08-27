using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Pets;
using PetBoarding_Api.Dto.Pets.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Pets;
using PetBoarding_Application.Pets.UpdatePet;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Api.Endpoints.Pets;

public static partial class PetsEndpoints
{
    private static async Task<IResult> UpdatePet(
        [FromRoute] Guid id,
        [FromBody] UpdatePetRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePetCommand(
            PetId: new PetId(id),
            Name: request.Name,
            Type: request.Type,
            Breed: request.Breed,
            Age: request.Age,
            Color: request.Color,
            Gender: request.Gender,
            IsNeutered: request.IsNeutered,
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
            PetResponseMapper.ToUpdatePetResponse);
    }
}