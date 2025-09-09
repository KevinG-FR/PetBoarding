using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Users;

public class UserId : EntityIdentifier
{
    [JsonConstructor]
    public UserId(Guid Value)
        : base(Value) { }
}