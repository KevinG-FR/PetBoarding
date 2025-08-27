using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Errors.Entities;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.DeletePet;

public sealed class DeletePetCommandHandler : ICommandHandler<DeletePetCommand>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdAsync(request.PetId, cancellationToken);
        
        if (pet is null)
        {
            return Result.Fail(new EntityNotFoundError(nameof(Pet), request.PetId.Value));
        }

        _petRepository.Remove(pet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}