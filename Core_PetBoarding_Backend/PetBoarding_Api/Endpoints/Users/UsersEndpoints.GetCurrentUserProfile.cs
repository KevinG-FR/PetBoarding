using MediatR;
using Microsoft.AspNetCore.Authorization;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Core.Users.GetUserById;
using PetBoarding_Domain.Users;
using System.Security.Claims;

namespace PetBoarding_Api.Endpoints.Users;

public static partial class UsersEndpoints
{
    [Authorize]
    private static async Task<IResult> GetCurrentUserProfile(
        ClaimsPrincipal user,
        ISender sender)
    {         
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var userResult = await sender.Send(new GetUserByIdQuery(new UserId(userId)));

        return userResult.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetUserResponse);
    }
}
