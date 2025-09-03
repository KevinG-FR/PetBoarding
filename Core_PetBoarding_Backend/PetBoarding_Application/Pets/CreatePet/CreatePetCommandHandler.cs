using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.CreatePet;

public sealed class CreatePetCommandHandler : ICommandHandler<CreatePetCommand, Pet>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreatePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<Pet>> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Créer le contact d'urgence si les informations sont fournies
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

            // Créer l'animal
            var pet = new Pet(
                name: request.Name,
                type: request.Type,
                breed: request.Breed,
                age: request.Age,
                color: request.Color,
                gender: request.Gender,
                isNeutered: request.IsNeutered,
                ownerId: request.OwnerId,
                weight: request.Weight,
                microchipNumber: request.MicrochipNumber,
                medicalNotes: request.MedicalNotes,
                specialNeeds: request.SpecialNeeds,
                photoUrl: request.PhotoUrl,
                emergencyContact: emergencyContact);

            await _petRepository.AddAsync(pet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await _cacheService.RemoveAsync(CacheKeys.Pets.ByOwner(request.OwnerId.Value), cancellationToken);

            return Result.Ok(pet);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail($"Erreur de validation : {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Erreur lors de la création de l'animal : {ex.Message}");
        }
    }
}