using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.GetPetsByOwner;

public sealed class GetPetsByOwnerQueryHandler : IQueryHandler<GetPetsByOwnerQuery, IEnumerable<Pet>>
{
    private readonly IPetRepository _petRepository;

    public GetPetsByOwnerQueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<Result<IEnumerable<Pet>>> Handle(GetPetsByOwnerQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Pet> pets;

        if (request.Type.HasValue)
        {
            pets = await _petRepository.GetByTypeAndOwnerAsync(
                request.Type.Value, 
                request.OwnerId, 
                cancellationToken);
        }
        else
        {
            pets = await _petRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        }

        return Result.Ok(pets);
    }
}