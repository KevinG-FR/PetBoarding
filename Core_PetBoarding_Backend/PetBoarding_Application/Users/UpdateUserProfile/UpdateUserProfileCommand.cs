using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.UpdateUserProfile
{
    public record UpdateUserProfileCommand(
        UserId UserId,
        string Firstname,
        string Lastname,
        string PhoneNumber) : ICommand<User>;
}
