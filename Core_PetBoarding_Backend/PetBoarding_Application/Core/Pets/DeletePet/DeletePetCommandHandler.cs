using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.DeletePet;

public sealed class DeletePetCommandHandler : ICommandHandler<DeletePetCommand, bool>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeletePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<bool>> Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdAsync(request.PetId, cancellationToken);
        
        if (pet is null)
        {
            return Result.Fail<bool>("Pet not found");
        }

        var petId = pet.Id.Value;
        var ownerId = pet.OwnerId.Value;

        var deleteResult = await _petRepository.DeleteAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cacheService.RemoveAsync(CacheKeys.Pets.ById(petId), cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.Pets.ByOwner(ownerId), cancellationToken);

        return Result.Ok(deleteResult > 0);
    }
}