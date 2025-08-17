namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;

public sealed record PlanningId : EntityIdentifier
{
    public PlanningId(Guid value) : base(value)
    {
    }

    public static PlanningId New() => new(Guid.NewGuid());
}
