namespace PetBoarding_Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Payments;

public sealed class PaymentRepository : BaseRepository<Payment, PaymentId>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Payment?> GetByExternalTransactionIdAsync(string externalTransactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.ExternalTransactionId == externalTransactionId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == PaymentStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == PaymentStatus.Failed)
            .ToListAsync(cancellationToken);
    }
}