namespace PetBoarding_Application.Core.Prestations.GetPrestationById;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class GetPrestationByIdQueryHandler : IQueryHandler<GetPrestationByIdQuery, Prestation>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly ICacheService _cacheService;

    public GetPrestationByIdQueryHandler(IPrestationRepository prestationRepository, ICacheService cacheService)
    {
        _prestationRepository = prestationRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<Prestation>> Handle(GetPrestationByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(query.Id, out var prestationId))
            {
                return Result.Fail("Invalid prestation ID format");
            }

            var cacheKey = CacheKeys.Prestations.ById(prestationId);

            // Get from cache or create if not exists
            var prestation = await _cacheService.GetOrCreateAsync<Prestation>(cacheKey, async () =>
            {
                var prestationFromDb = await _prestationRepository.GetByIdAsync(new PrestationId(prestationId), cancellationToken);
                return prestationFromDb;
            }, TimeSpan.FromHours(2), cancellationToken);

            if (prestation is null)
            {
                return Result.Fail("Prestation not found");
            }

            return Result.Ok(prestation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving prestation: {ex.Message}");
        }
    }
}
