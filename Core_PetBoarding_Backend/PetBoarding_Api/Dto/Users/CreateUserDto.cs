using PetBoarding_Domain.Users;

namespace PetBoarding_Api.Dto.Users
{
    public record CreateUserDto
    {
        public string Firstname { get; init; } = string.Empty;
        public string Lastname { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public UserProfileType ProfileType { get; init; }
    }   
}
