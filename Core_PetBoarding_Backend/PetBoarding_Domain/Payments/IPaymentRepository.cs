namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Abstractions;

public interface IPaymentRepository : IBaseRepository<Payment, PaymentId>
{
    Task<Payment?> GetByExternalTransactionIdAsync(string externalTransactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetFailedPaymentsAsync(CancellationToken cancellationToken = default);
}