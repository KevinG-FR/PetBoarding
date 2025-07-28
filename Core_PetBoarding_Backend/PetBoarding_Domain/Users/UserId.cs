using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Users
{
    public record UserId : EntityIdentifier
    {
        public UserId(Guid Value)
            : base(Value) { }
    }
}