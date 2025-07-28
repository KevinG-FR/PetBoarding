using PetBoarding_Domain.Users;

namespace PetBoarding_Api.Dto.Users
{
    public record CreateUserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public UserProfileType ProfileType { get; set; }

        public CreateUserDto(string firstname, string lastname, string email, string passwordHash, string phoneNumber, UserProfileType profileType)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
            ProfileType = profileType;
        }
    }
}
