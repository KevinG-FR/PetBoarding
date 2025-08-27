using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Repositories;

internal sealed class PetRepository : BaseRepository<Pet, PetId>, IPetRepository
{
    public PetRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public new async Task<IEnumerable<Pet>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetByOwnerIdAsync(
        UserId ownerId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Where(p => p.OwnerId == ownerId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetByTypeAsync(
        PetType type, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Where(p => p.Type == type)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetByTypeAndOwnerAsync(
        PetType type, 
        UserId ownerId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Where(p => p.Type == type && p.OwnerId == ownerId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pet?> GetByIdWithOwnerAsync(
        PetId petId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == petId, cancellationToken);
    }
}