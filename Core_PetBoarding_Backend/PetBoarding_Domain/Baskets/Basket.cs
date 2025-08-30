namespace PetBoarding_Domain.Baskets;

using FluentResults;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

public sealed class Basket : Entity<BasketId>, IAuditableEntity
{
    private const int MAX_PAYMENT_FAILURES = 3;
    
    private readonly List<BasketItem> _items = new();

    public Basket(UserId userId) : base(new BasketId(Guid.CreateVersion7()))
    {
        UserId = userId;
        Status = BasketStatus.Created;
        PaymentFailureCount = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private set; }
    public User? User { get; private set; }
    public BasketStatus Status { get; private set; }
    public PaymentId? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }
    public int PaymentFailureCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public Result AddReservation(ReservationId reservationId)
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot add reservations to basket with status {Status.Name}");

        var existingItem = _items.FirstOrDefault(i => i.ReservationId == reservationId);
        
        if (existingItem != null)
            return Result.Fail("Reservation is already in basket");

        var newItem = new BasketItem(Id, reservationId);
        _items.Add(newItem);

        UpdateTimestamp();
        return Result.Ok();
    }

    public Result RemoveReservation(ReservationId reservationId)
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot remove reservations from basket with status {Status.Name}");

        var item = _items.FirstOrDefault(i => i.ReservationId == reservationId);
        if (item == null)
            return Result.Fail("Reservation not found in basket");

        _items.Remove(item);
        UpdateTimestamp();

        if (!_items.Any())
        {
            Cancel();
        }

        return Result.Ok();
    }

    public Result ClearItems()
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot clear basket with status {Status.Name}");

        _items.Clear();
        UpdateTimestamp();
        Cancel();

        return Result.Ok();
    }

    public decimal GetTotalAmount()
    {
        return _items.Sum(item => item.GetTotalPrice());
    }

    public int GetTotalItemCount()
    {
        return _items.Count;
    }

    public bool IsEmpty() => !_items.Any();

    public Result AssignPayment(PaymentId paymentId)
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot assign payment to basket with status {Status.Name}");

        if (IsEmpty())
            return Result.Fail("Cannot assign payment to empty basket");

        PaymentId = paymentId;
        UpdateTimestamp();

        return Result.Ok();
    }

    public Result MarkAsPaid()
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot mark basket as paid with status {Status.Name}");

        if (PaymentId == null)
            return Result.Fail("Cannot mark basket as paid without assigned payment");

        Status = BasketStatus.Paid;
        PaymentFailureCount = 0;
        UpdateTimestamp();

        return Result.Ok();
    }

    public Result RecordPaymentFailure()
    {
        if (Status != BasketStatus.Created)
            return Result.Fail($"Cannot record payment failure for basket with status {Status.Name}");

        PaymentFailureCount++;

        if (PaymentFailureCount >= MAX_PAYMENT_FAILURES)
        {
            Status = BasketStatus.Cancelled;
        }
        else
        {
            Status = BasketStatus.PaymentFailure;
        }

        UpdateTimestamp();

        return Result.Ok();
    }

    public Result RetryPayment()
    {
        if (Status != BasketStatus.PaymentFailure)
            return Result.Fail($"Cannot retry payment for basket with status {Status.Name}");

        if (PaymentFailureCount >= MAX_PAYMENT_FAILURES)
            return Result.Fail("Maximum payment failures reached, basket is cancelled");

        Status = BasketStatus.Created;
        UpdateTimestamp();

        return Result.Ok();
    }

    public void Cancel()
    {
        if (Status == BasketStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid basket");

        Status = BasketStatus.Cancelled;
        UpdateTimestamp();
    }

    public bool CanBeModified() => Status == BasketStatus.Created;
    public bool RequiresPayment() => Status == BasketStatus.Created && !IsEmpty();
    public bool IsPaymentInProgress() => PaymentId != null && Status == BasketStatus.Created;
    
    public IEnumerable<ReservationId> GetReservationIds()
    {
        return _items.Select(item => item.ReservationId);
    }

    /// <summary>
    /// Marks the basket as paid and returns the reservation IDs that need to be updated
    /// </summary>
    public Result<IEnumerable<ReservationId>> MarkAsPaidAndGetReservations()
    {
        var markAsPaidResult = MarkAsPaid();
        if (markAsPaidResult.IsFailed)
            return Result.Fail<IEnumerable<ReservationId>>(markAsPaidResult.Errors);

        return Result.Ok(GetReservationIds());
    }
}