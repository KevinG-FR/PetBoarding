using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;

namespace PetBoarding_Application.Core.Pets.GetPetsByOwner;

public sealed class GetPetsByOwnerQueryHandler : IQueryHandler<GetPetsByOwnerQuery, IEnumerable<Pet>>
{
    private readonly IPetRepository _petRepository;
    private readonly ICacheService _cacheService;

    public GetPetsByOwnerQueryHandler(IPetRepository petRepository, ICacheService cacheService)
    {
        _petRepository = petRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<Pet>>> Handle(GetPetsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Pets.ByOwner(request.OwnerId.Value);

        // Get from cache or create if not exists
        var pets = await _cacheService.GetOrCreateAsync<List<Pet>>(cacheKey, async () =>
        {
            var petsFromDb = await _petRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
            return petsFromDb.ToList();
        }, TimeSpan.FromMinutes(45), cancellationToken);

        if (pets is null || !pets.Any())
        {
            return Result.Ok(Enumerable.Empty<Pet>());
        }

        // Apply type filter in memory if specified
        var filteredPets = request.Type.HasValue 
            ? pets.Where(p => p.Type == request.Type.Value) 
            : pets;

        return Result.Ok(filteredPets);
    }
}