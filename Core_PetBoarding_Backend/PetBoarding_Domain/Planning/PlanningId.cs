namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;

public sealed class PlanningId : EntityIdentifier
{
    public PlanningId(Guid value) : base(value)
    {
    }

    public static PlanningId New() => new(Guid.NewGuid());
}
