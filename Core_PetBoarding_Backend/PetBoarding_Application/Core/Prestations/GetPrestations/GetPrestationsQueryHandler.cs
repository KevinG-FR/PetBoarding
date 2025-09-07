namespace PetBoarding_Application.Core.Prestations.GetPrestations;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class GetPrestationsQueryHandler : IQueryHandler<GetPrestationsQuery, IEnumerable<Prestation>>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly ICacheService _cacheService;

    public GetPrestationsQueryHandler(IPrestationRepository prestationRepository, ICacheService cacheService)
    {
        _prestationRepository = prestationRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<Prestation>>> Handle(GetPrestationsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = CacheKeys.Prestations.AllPrestations();

            var _ = await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            // Get from cache or create if not exists
            var prestations = await _cacheService.GetOrCreateAsync<List<Prestation>>(cacheKey, async () =>
            {
                var prestationsFromDb = await _prestationRepository.GetAllAsync(cancellationToken);
                return prestationsFromDb;
            }, TimeSpan.FromHours(2), cancellationToken);

            //var prestations = await _prestationRepository.GetAllAsync(cancellationToken);

            if (prestations is null)
            {
                return Result.Fail("No prestations found");
            }

            var filteredPrestations = prestations.AsEnumerable();

            // Filtrage par catégorie d'animal
            if (query.CategorieAnimal.HasValue)
            {
                filteredPrestations = filteredPrestations.Where(p => p.CategorieAnimal == query.CategorieAnimal.Value);
            }

            // Filtrage par disponibilité
            if (query.EstDisponible.HasValue)
            {
                filteredPrestations = filteredPrestations.Where(p => p.EstDisponible == query.EstDisponible.Value);
            }

            // Filtrage par texte de recherche (libellé ou description)
            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var searchText = query.SearchText.ToLowerInvariant();
                filteredPrestations = filteredPrestations.Where(p => 
                    p.Libelle.ToLowerInvariant().Contains(searchText) ||
                    p.Description.ToLowerInvariant().Contains(searchText));
            }

            return Result.Ok(filteredPrestations);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving prestations: {ex.Message}");
        }
    }
}
