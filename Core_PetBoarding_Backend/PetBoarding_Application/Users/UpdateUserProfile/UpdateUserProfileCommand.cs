using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.UpdateUserProfile
{
    public record UpdateUserProfileCommand(
        UserId UserId,
        string Firstname,
        string Lastname,
        string PhoneNumber,
        AddressData? Address = null) : ICommand<User>;

    public record AddressData(
        string StreetNumber,
        string StreetName,
        string City,
        string PostalCode,
        string Country,
        string? Complement = null);
}
