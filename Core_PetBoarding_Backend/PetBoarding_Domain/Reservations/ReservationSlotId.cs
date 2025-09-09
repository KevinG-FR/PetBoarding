using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Reservations;

/// <summary>
/// Strong-typed identifier for ReservationSlot entity
/// </summary>
public sealed class ReservationSlotId : EntityIdentifier
{
    public ReservationSlotId(Guid value) : base(value)
    {
    }

    public static ReservationSlotId New() => new(Guid.CreateVersion7());
}