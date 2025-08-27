using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Errors.Entities;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.GetPetById;

public sealed class GetPetByIdQueryHandler : IQueryHandler<GetPetByIdQuery, Pet>
{
    private readonly IPetRepository _petRepository;

    public GetPetByIdQueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<Result<Pet>> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdWithOwnerAsync(request.PetId, cancellationToken);

        if (pet is null)
        {
            return Result.Fail(new EntityNotFoundError(nameof(Pet), request.PetId.Value));
        }

        return Result.Ok(pet);
    }
}