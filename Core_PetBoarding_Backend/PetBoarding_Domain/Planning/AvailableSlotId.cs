namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;

public sealed class AvailableSlotId : EntityIdentifier
{
    public AvailableSlotId(Guid value) : base(value)
    {
    }

    public static AvailableSlotId New() => new(Guid.CreateVersion7());
}