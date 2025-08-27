using MediatR;
using Microsoft.AspNetCore.Authorization;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Users.GetUserById;
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
        Console.WriteLine($"🔍 GetCurrentUserProfile called");
        Console.WriteLine($"👤 User.Identity.IsAuthenticated: {user.Identity?.IsAuthenticated}");
        Console.WriteLine($"📋 Claims count: {user.Claims.Count()}");
        
        foreach (var claim in user.Claims)
        {
            Console.WriteLine($"   - {claim.Type}: {claim.Value}");
        }
        
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"🆔 NameIdentifier claim: {userIdClaim}");
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            Console.WriteLine($"❌ Invalid or missing NameIdentifier claim");
            return Results.Unauthorized();
        }

        Console.WriteLine($"✅ Valid UserId: {userId}");
        var userResult = await sender.Send(new GetUserByIdQuery(new UserId(userId)));

        return userResult.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetUserResponse);
    }
}
