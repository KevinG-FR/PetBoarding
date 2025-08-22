namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Users;
using PetBoarding_Api.Dto.Users.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Users.UpdateUserProfile;
using PetBoarding_Domain.Users;

public static partial class UsersEndpoints
{
    private static async Task<IResult> UpdateUserProfile(
        Guid userId,
        [FromBody] UpdateUserDto updateDto,
        ISender sender)
    {
        AddressData? addressData = null;
        if (updateDto.Address != null)
        {
            addressData = new AddressData(
                updateDto.Address.StreetNumber,
                updateDto.Address.StreetName,
                updateDto.Address.City,
                updateDto.Address.PostalCode,
                updateDto.Address.Country,
                updateDto.Address.Complement);
        }

        var command = new UpdateUserProfileCommand(
            new UserId(userId),
            updateDto.Firstname,
            updateDto.Lastname,
            updateDto.PhoneNumber,
            addressData);

        var result = await sender.Send(command);

        return result.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetUserResponse);
    }
}
