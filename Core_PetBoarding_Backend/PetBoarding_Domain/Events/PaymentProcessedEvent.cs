using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Reservations;

namespace PetBoarding_Domain.Events;

public sealed class PaymentProcessedEvent : DomainEvent
{
    public PaymentProcessedEvent(PaymentId paymentId, UserId userId, ReservationId? reservationId, 
        decimal amount, string status, string paymentMethod)
    {
        PaymentId = paymentId;
        UserId = userId;
        ReservationId = reservationId;
        Amount = amount;
        Status = status;
        PaymentMethod = paymentMethod;
    }

    public PaymentId PaymentId { get; private init; }
    public UserId UserId { get; private init; }
    public ReservationId? ReservationId { get; private init; }
    public decimal Amount { get; private init; }
    public string Status { get; private init; }
    public string PaymentMethod { get; private init; }
}