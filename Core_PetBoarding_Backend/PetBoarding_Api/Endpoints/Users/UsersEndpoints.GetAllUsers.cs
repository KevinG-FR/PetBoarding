namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Core.Users.GetAllUsers;
using PetBoarding_Domain.Accounts;
using PetBoarding_Infrastructure.Authentication;

public static partial class UsersEndpoints
{
    [HasPermission(Permission.ReadMember)]
    private static async Task<IResult> GetAllUsers(ISender sender)
    {
        var getAllUsersResult = await sender.Send(new GetAllUsersQuery());

        return getAllUsersResult.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetAllUsersResponse);
    }
}
