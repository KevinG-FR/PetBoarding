using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors.Entities;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.UpdatePet;

public sealed class UpdatePetCommandHandler : ICommandHandler<UpdatePetCommand, Pet>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdatePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<Pet>> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pet = await _petRepository.GetByIdAsync(request.PetId, cancellationToken);
            
            if (pet is null)
            {
                return Result.Fail("Pet not found");
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

            var updatedPet = await _petRepository.UpdateAsync(pet, cancellationToken);
            if (updatedPet is null)
            {
                return Result.Fail("Error occurred while updating the pet");
            }

            // Invalidate cache
           await _cacheService.SetAsync(
                    CacheKeys.Pets.ById(request.PetId.Value), 
                    updatedPet, 
                    TimeSpan.FromHours(1), 
                    cancellationToken);
                    
            // Mettre à jour le cache de la liste du propriétaire
            var ownerPets = await _petRepository.GetByOwnerIdAsync(updatedPet.OwnerId, cancellationToken);
            
            await _cacheService.SetAsync(
                CacheKeys.Pets.ByOwner(updatedPet.OwnerId.Value),
                ownerPets.ToList(),
                TimeSpan.FromMinutes(45),
                cancellationToken);

            return Result.Ok(updatedPet);
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