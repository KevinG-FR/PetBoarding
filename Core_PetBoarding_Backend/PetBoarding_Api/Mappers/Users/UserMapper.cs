using PetBoarding_Api.Dto.Addresses;
using PetBoarding_Domain.Users;

namespace PetBoarding_Api.Mappers.Users;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException($"User is null");
        }

        // ID peut être null à cause du constructeur EF parameterless - on utilise Guid.Empty comme fallback

        AddressDto? addressDto = null;
        if (user.Address is not null)
        {
            addressDto = new AddressDto(
                user.Address.StreetNumber.Value,
                user.Address.StreetName.Value,
                user.Address.City.Value,
                user.Address.PostalCode.Value,
                user.Address.Country.Value,
                user.Address.Complement?.Value);
        }

        return new UserDto
        {
            Id = user.Id?.Value ?? Guid.Empty,
            Email = user.Email.Value,
            FirstName = user.Firstname.Value,
            LastName = user.Lastname.Value,
            PhoneNumber = user.PhoneNumber.Value,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            ProfileType = user.ProfileType.ToString(),
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Address = addressDto
        };
    }             
}
