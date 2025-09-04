using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Events;

public sealed class ReservationStatusChangedEvent : DomainEvent
{
    public ReservationStatusChangedEvent(ReservationId reservationId, UserId userId, string oldStatus, string newStatus, string? reason = null)
    {
        ReservationId = reservationId;
        UserId = userId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
    }

    public ReservationId ReservationId { get; private init; }
    public UserId UserId { get; private init; }
    public string OldStatus { get; private init; }
    public string NewStatus { get; private init; }
    public string? Reason { get; private init; }
}