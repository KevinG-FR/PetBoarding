namespace PetBoarding_Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Prestations;

internal sealed class PrestationRepository : BaseRepository<Prestation, PrestationId>, IPrestationRepository
{
    public PrestationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Prestation>> GetAllAsync(
        TypeAnimal? categorieAnimal = null,
        bool? estDisponible = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (categorieAnimal.HasValue)
        {
            query = query.Where(p => p.CategorieAnimal == categorieAnimal.Value);
        }

        if (estDisponible.HasValue)
        {
            query = query.Where(p => p.EstDisponible == estDisponible.Value);
        }

        return await query
            .OrderByDescending(p => p.DateCreation)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestation>> GetByCategorieAnimalAsync(
        TypeAnimal categorieAnimal, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CategorieAnimal == categorieAnimal)
            .OrderBy(p => p.Libelle)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prestation>> GetDisponiblesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.EstDisponible)
            .OrderBy(p => p.CategorieAnimal)
            .ThenBy(p => p.Libelle)
            .ToListAsync(cancellationToken);
    }
}
