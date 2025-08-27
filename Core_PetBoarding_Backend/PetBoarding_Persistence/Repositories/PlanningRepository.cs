namespace PetBoarding_Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
public sealed class PlanningRepository : BaseRepository<Planning, PlanningId>, IPlanningRepository
{
    public PlanningRepository(ApplicationDbContext _context) : base(_context)
    {
    }

    public async Task<Planning?> GetByPrestationIdAsync(PrestationId prestationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.PrestationId == prestationId, cancellationToken);
    }

    public async Task<List<Planning>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AvailableSlot>> GetCreneauxPourMoisAsync(PrestationId prestationId, int annee, int mois, CancellationToken cancellationToken = default)
    {
        var planning = await GetByPrestationIdAsync(prestationId, cancellationToken);
        
        if (planning == null)
            return new List<AvailableSlot>();

        return planning.Creneaux
            .Where(c => c.Date.Year == annee && c.Date.Month == mois)
            .ToList();
    }

    public async Task<bool> ExistsByPrestationIdAsync(PrestationId prestationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.PrestationId == prestationId, cancellationToken);
    }
}