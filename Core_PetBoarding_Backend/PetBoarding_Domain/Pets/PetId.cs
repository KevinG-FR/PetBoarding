using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Pets;

public record PetId : EntityIdentifier
{
    public PetId(Guid Value)
        : base(Value) { }
}