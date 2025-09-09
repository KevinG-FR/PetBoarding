using PetBoarding_Api.Dto.Addresses;

namespace PetBoarding_Api.Dto.Users;

public record UpdateUserDto
{
    public string Firstname { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public AddressDto? Address { get; init; }
}
