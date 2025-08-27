using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors.Entities;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.UpdatePet;

public sealed class UpdatePetCommandHandler : ICommandHandler<UpdatePetCommand, Pet>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Pet>> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(request.PetId, cancellationToken);
            
            if (pet is null)
            {
                return Result.Fail(new EntityNotFoundError(nameof(Pet), request.PetId.Value));
            }

            // Mettre à jour les informations de base
            pet.UpdateBasicInfo(request.Name, request.Breed, request.Age, request.Color);
            pet.UpdateType(request.Type);
            pet.UpdateGender(request.Gender);
            pet.UpdateNeuteredStatus(request.IsNeutered);
            pet.UpdateWeight(request.Weight);
            pet.UpdateMicrochipNumber(request.MicrochipNumber);
            pet.UpdateMedicalNotes(request.MedicalNotes);
            pet.UpdateSpecialNeeds(request.SpecialNeeds);
            pet.UpdatePhotoUrl(request.PhotoUrl);

            // Mettre à jour le contact d'urgence
            EmergencyContact? emergencyContact = null;
            if (!string.IsNullOrWhiteSpace(request.EmergencyContactName) &&
                !string.IsNullOrWhiteSpace(request.EmergencyContactPhone) &&
                !string.IsNullOrWhiteSpace(request.EmergencyContactRelationship))
            {
                emergencyContact = new EmergencyContact(
                    request.EmergencyContactName,
                    request.EmergencyContactPhone,
                    request.EmergencyContactRelationship);
            }
            pet.UpdateEmergencyContact(emergencyContact);

            _petRepository.Update(pet);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(pet);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail($"Erreur de validation : {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Erreur lors de la mise à jour de l'animal : {ex.Message}");
        }
    }
}