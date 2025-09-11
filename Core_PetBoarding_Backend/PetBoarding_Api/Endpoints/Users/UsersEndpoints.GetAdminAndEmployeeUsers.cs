namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using Microsoft.AspNetCore.Http;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Core.Users.GetAdminAndEmployeeUsers;
using PetBoarding_Domain.Users;
using System.Security.Claims;

public static partial class UsersEndpoints
{
    private static async Task<IResult> GetAdminAndEmployeeUsers(ISender sender, HttpContext httpContext, int? profileType = null)
    {
        // Récupérer l'ID de l'utilisateur actuel depuis les claims
        var currentUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }

        var getUsersResult = await sender.Send(new GetAdminAndEmployeeUsersQuery(
            CurrentUserId: new UserId(currentUserId), 
            ProfileTypeFilter: profileType));

        return getUsersResult.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetAllUsersResponse);
    }
}