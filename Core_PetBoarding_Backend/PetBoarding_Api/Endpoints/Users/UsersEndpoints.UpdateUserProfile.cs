namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Users;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Core.Users.GetUserById;
using PetBoarding_Application.Core.Users.UpdateUserProfile;
using PetBoarding_Domain.Users;
using System.Security.Claims;

public static partial class UsersEndpoints
{
    private static async Task<IResult> UpdateUserProfile(
        Guid userId,
        [FromBody] UpdateUserDto updateDto,
        ISender sender,
        ClaimsPrincipal user)
    {
        // Security verification: Ensure user can only modify their own profile or is an administrator
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var currentUserIdParsed))
        {
            return Results.Unauthorized();
        }

        // Allow modification if user is updating their own profile
        bool isOwnProfile = currentUserIdParsed == userId;
        
        // Check if user is administrator (for modifying other profiles)
        bool isAdministrator = false;
        if (!isOwnProfile)
        {
            // We need to get the current user's profile type to check if they're an administrator
            var currentUserResult = await sender.Send(new GetUserByIdQuery(new UserId(currentUserIdParsed)));
            if (currentUserResult.IsSuccess && currentUserResult.Value.ProfileType == UserProfileType.Administrator)
            {
                isAdministrator = true;
            }
        }

        // Deny access if user is not modifying their own profile and is not an administrator
        if (!isOwnProfile && !isAdministrator)
        {
            return Results.Forbid();
        }

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
            updateDto.Email,
            updateDto.PhoneNumber,
            addressData);

        var result = await sender.Send(command);

        return result.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetUserResponse);
    }
}
