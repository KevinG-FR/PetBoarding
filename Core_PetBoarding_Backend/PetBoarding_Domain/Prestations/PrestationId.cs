namespace PetBoarding_Domain.Prestations;

using PetBoarding_Domain.Abstractions;

public sealed class PrestationId : EntityIdentifier
{
    public PrestationId(Guid value) : base(value)
    {
    }

    public static PrestationId New() => new(Guid.CreateVersion7());
}
