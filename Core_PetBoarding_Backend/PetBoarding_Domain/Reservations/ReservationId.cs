namespace PetBoarding_Domain.Reservations;

using PetBoarding_Domain.Abstractions;

public sealed record ReservationId : EntityIdentifier
{
    public ReservationId(Guid value) : base(value)
    {
    }
}
