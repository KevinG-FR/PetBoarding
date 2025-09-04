namespace PetBoarding_Domain.Payments;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Events;
using PetBoarding_Domain.Users;

public sealed class Payment : EntityWithDomainEvents<PaymentId>, IAuditableEntity
{
    public Payment(
        decimal amount,
        PaymentMethod method,
        string? externalTransactionId = null,
        string? description = null) : base(new PaymentId(Guid.CreateVersion7()))
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        ExternalTransactionId = externalTransactionId;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? ExternalTransactionId { get; private set; }
    public string? Description { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureReason { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSuccess(UserId userId, string? externalTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark payment as success when status is {Status}");

        Status = PaymentStatus.Success;
        ProcessedAt = DateTime.UtcNow;
        ExternalTransactionId = externalTransactionId ?? ExternalTransactionId;
        FailureReason = null;

        AddDomainEvent(new PaymentProcessedEvent(
            Id, 
            userId, 
            null, // ReservationId - can be added later if needed
            Amount, 
            Status.ToString(), 
            Method.ToString()));
    }

    public void MarkAsFailed(string failureReason)
    {
        if (Status == PaymentStatus.Success)
            throw new InvalidOperationException("Cannot mark successful payment as failed");

        Status = PaymentStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
        FailureReason = failureReason;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        if (Status == PaymentStatus.Success)
            throw new InvalidOperationException("Cannot cancel successful payment");

        Status = PaymentStatus.Cancelled;
        ProcessedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsSuccessful() => Status == PaymentStatus.Success;
    public bool IsFailed() => Status == PaymentStatus.Failed;
    public bool IsPending() => Status == PaymentStatus.Pending;
    public bool IsCancelled() => Status == PaymentStatus.Cancelled;
}