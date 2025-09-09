using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Errors;
using PetBoarding_Domain.Events;

namespace PetBoarding_Domain.Users;

public class User : AuditableEntityWithDomainEvents<UserId>
{
    // Constructeur privé pour création d'un nouvel utilisateur
    private User(Firstname firstname, Lastname lastname, Email email, PhoneNumber phoneNumber, string passwordHash, UserProfileType profileType)
        : base(new UserId(Guid.CreateVersion7()))
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        ProfileType = profileType;
        Status = UserStatus.Created;

        AddDomainEvent(new UserRegisteredEvent(
            Id,
            firstname.Value,
            lastname.Value,
            email.Value,
            DateTime.UtcNow));
    }

    // Constructeur privé pour Entity Framework (reconstruction depuis la DB)
    private User() : base(default!) { }

    // Méthode factory pour créer un nouvel utilisateur (explicite)
    public static User Create(Firstname firstname, Lastname lastname, Email email, PhoneNumber phoneNumber, string passwordHash, UserProfileType profileType)
    {
        return new User(firstname, lastname, email, phoneNumber, passwordHash, profileType);
    }

    public Firstname Firstname { get; set; }

    public Lastname Lastname { get; set; }

    public Email Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public PhoneNumber PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public AddressId? AddressId { get; set; }

    public Address? Address { get; set; }

    public UserProfileType ProfileType { get; private set; }
    public UserStatus Status { get; private set; }                

    public Result ChangeForConfirmedStatus()
    {
        if (Status == UserStatus.Confirmed)
            return Result.Ok();

        if (Status != UserStatus.Created)
            return Result.Fail(UserErrors.ChangeUserStatus(Status, UserStatus.Confirmed));

        Status = UserStatus.Confirmed;

        return Result.Ok();
    }

    public Result ChangeForInactiveStatus()
    {
        if (Status == UserStatus.Inactive)
            return Result.Ok();

        if (Status != UserStatus.Created || Status != UserStatus.Confirmed)
            return Result.Fail(UserErrors.ChangeUserStatus(Status, UserStatus.Inactive));

        Status = UserStatus.Inactive;

        return Result.Ok();
    }

    public Result ChangeForDeletedStatus()
    {
        if (Status == UserStatus.Deleted)
            return Result.Ok();

        Status = UserStatus.Deleted;

        return Result.Ok();
    }

    public Result UpdateProfile(Firstname firstname, Lastname lastname, PhoneNumber phoneNumber, Address? address = null)
    {
        // Validation des données
        if (firstname == null || lastname == null || phoneNumber == null)
            return Result.Fail("Les données du profil ne peuvent pas être nulles");

        // Mise à jour des propriétés
        Firstname = firstname;
        Lastname = lastname;
        PhoneNumber = phoneNumber;
        
        // Mise à jour de l'adresse si fournie
        if (address is not null)
        {
            Address = address;
            AddressId = address.Id;
        }

        return Result.Ok();
    }
}
