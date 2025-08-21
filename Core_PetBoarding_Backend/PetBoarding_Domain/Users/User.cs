using FluentResults;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Errors;

namespace PetBoarding_Domain.Users
{
    public class User : Entity<UserId>, IAuditableEntity
    {
        public User(Firstname firstname, Lastname lastname, Email email, PhoneNumber phoneNumber, string passwordHash, UserProfileType profileType)
            : base(new UserId(Guid.CreateVersion7()))
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
            ProfileType = profileType;
            Status = UserStatus.Created;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
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

        public UserProfileType ProfileType { get; }
        public UserStatus Status { get; private set; }

        public readonly DateTime CreatedAt;
        public DateTime UpdatedAt { get; private set; }

        // Implémentation explicite de l'interface pour le readonly field
        DateTime IAuditableEntity.CreatedAt => CreatedAt;

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

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
            
            // Mise à jour du timestamp
            UpdateTimestamp();

            return Result.Ok();
        }
    }
}
