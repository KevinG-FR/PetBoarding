namespace PetBoarding_Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Reservations;

internal sealed class ReservationRepository : BaseRepository<Reservation, ReservationId>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Reservation?> GetByIdAsync(ReservationId entityIdentifier, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(x => x.ReservedSlots).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync(
        string? userId = null,
        string? serviceId = null,
        ReservationStatus? status = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(r => r.UserId == userId);
        }

        if (!string.IsNullOrEmpty(serviceId))
        {
            query = query.Where(r => r.ServiceId == serviceId);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (startDateMin.HasValue)
        {
            query = query.Where(r => r.StartDate >= startDateMin.Value);
        }

        if (startDateMax.HasValue)
        {
            query = query.Where(r => r.StartDate <= startDateMax.Value);
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }    

    public async Task<IEnumerable<Reservation>> GetByServiceIdAsync(
        string serviceId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.ServiceId == serviceId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsBetweenDatesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => 
                (r.StartDate <= endDate) && 
                (r.EndDate == null || r.EndDate >= startDate || r.StartDate >= startDate))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetExpiredCreatedReservationsAsync(
        CancellationToken cancellationToken = default)
    {
        // Payment expiry concept has been removed - reservations no longer expire automatically
        // Return empty list to maintain interface compatibility
        return new List<Reservation>();
    }

    public async Task<IEnumerable<Reservation>> GetUserDisplayedReservationsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var displayedStatuses = new[] 
        {
            ReservationStatus.Validated,
            ReservationStatus.InProgress,
            ReservationStatus.Completed
        };

        return await _dbSet
            .Where(r => r.UserId == userId && displayedStatuses.Contains(r.Status))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}