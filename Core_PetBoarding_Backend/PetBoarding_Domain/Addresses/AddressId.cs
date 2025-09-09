using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Addresses;

public class AddressId : EntityIdentifier
{
    public AddressId(Guid Value)
        : base(Value) { }
}
