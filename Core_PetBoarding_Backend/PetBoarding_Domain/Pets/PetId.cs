using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Pets;

public class PetId : EntityIdentifier
{
    public PetId(Guid Value)
        : base(Value) { }
}