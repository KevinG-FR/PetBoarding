namespace PetBoarding_Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Users;

public sealed class BasketRepository : BaseRepository<Basket, BasketId>, IBasketRepository
{
    public BasketRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Basket?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
    }

    public async Task<Basket?> GetByUserIdWithItemsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Items)
            .ThenInclude(i => i.Reservation)
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
    }

    public async Task<Basket?> GetByIdWithItemsAsync(BasketId basketId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Items)
            .ThenInclude(i => i.Reservation)
            .FirstOrDefaultAsync(b => b.Id == basketId, cancellationToken);
    }

    public async Task<IEnumerable<Basket>> GetExpiredBaskets(CancellationToken cancellationToken = default)
    {
        var expiredTime = DateTime.UtcNow.AddHours(-24);
        
        return await _dbSet
            .Where(b => b.Status == BasketStatus.Created && b.CreatedAt < expiredTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Basket>> GetBasketsWithPaymentFailures(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.Status == BasketStatus.PaymentFailure)
            .ToListAsync(cancellationToken);
    }
}