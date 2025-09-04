using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Events;

public sealed class UserRegisteredEvent : DomainEvent
{
    public UserRegisteredEvent(UserId userId, string firstName, string lastName, string email, DateTime registrationDate)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        RegistrationDate = registrationDate;
    }

    public UserId UserId { get; private init; }
    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public string Email { get; private init; }
    public DateTime RegistrationDate { get; private init; }
}