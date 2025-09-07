using PetBoarding_Application.Core.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Web.Account.CreateAccount
{
    public record CreateAccountCommand : ICommand<User>
    {
        public CreateAccountCommand(
            string email,
            string password,
            string firstname,
            string lastname,
            UserProfileType profileType,
            string phoneNumber)
        {
            Email = email;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            ProfileType = profileType;
            PhoneNumber = phoneNumber;
        }

        public string Email { get; }
        public string Password { get; }
        public string Firstname { get; }
        public string Lastname { get; }
        public UserProfileType ProfileType { get; }
        public string PhoneNumber { get; }
    }
}
