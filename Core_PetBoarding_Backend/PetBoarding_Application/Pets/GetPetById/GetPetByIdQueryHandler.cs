using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Pets.GetPetById;

public sealed class GetPetByIdQueryHandler : IQueryHandler<GetPetByIdQuery, Pet>
{
    private readonly IPetRepository _petRepository;
    private readonly ICacheService _cacheService;

    public GetPetByIdQueryHandler(IPetRepository petRepository, ICacheService cacheService)
    {
        _petRepository = petRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<Pet>> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Pets.ById(request.PetId.Value);

        // Get from cache or create if not exists
        var pet = await _cacheService.GetOrCreateAsync<Pet>(cacheKey, async () =>
        {
            var petFromDb = await _petRepository.GetByIdWithOwnerAsync(request.PetId, cancellationToken);
            return petFromDb;
        }, TimeSpan.FromHours(1), cancellationToken);

        if (pet is null)
        {
            return Result.Fail("Pet not found");
        }

        return Result.Ok(pet);
    }
}