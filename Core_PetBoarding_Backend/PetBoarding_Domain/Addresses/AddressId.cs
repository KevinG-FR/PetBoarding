using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Addresses;

public record AddressId : EntityIdentifier
{
    public AddressId(Guid Value)
        : base(Value) { }
}
