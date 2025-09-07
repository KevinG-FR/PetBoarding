using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.UpdateUserProfile;

public record AddressData(
    string StreetNumber,
    string StreetName, 
    string City,
    string PostalCode,
    string Country,
    string? Complement = null);

public record UpdateUserProfileCommand(
    UserId UserId,
    string Firstname,
    string Lastname,
    string PhoneNumber,
    AddressData? AddressData = null) : ICommand<User>;