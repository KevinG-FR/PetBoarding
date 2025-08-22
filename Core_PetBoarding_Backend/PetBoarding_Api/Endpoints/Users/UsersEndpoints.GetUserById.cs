namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using PetBoarding_Api.Dto.Users.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Users.GetUserById;
using PetBoarding_Domain.Users;

public static partial class UsersEndpoints
{
    private static async Task<IResult> GetUser(
        Guid userId,
        ISender sender)
    {
        var userResult = await sender.Send(new GetUserByIdQuery(new UserId(userId)));

        return userResult.GetHttpResult(
            UserMapper.ToDto, 
            UserResponseMapper.ToGetUserResponse);
    }
}
