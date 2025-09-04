using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Events;

public sealed class UserProfileUpdatedEvent : DomainEvent
{
    public UserProfileUpdatedEvent(UserId userId, string firstName, string lastName, string? phoneNumber)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public UserId UserId { get; private init; }
    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public string? PhoneNumber { get; private init; }
}