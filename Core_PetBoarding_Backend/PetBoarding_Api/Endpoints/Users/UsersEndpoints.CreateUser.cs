namespace PetBoarding_Api.Endpoints.Users;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Users;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Web.Account.CreateAccount;

public static partial class UsersEndpoints
{
    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserDto createUserDto,
        ISender sender)
    {
        var createUserCommand = new CreateAccountCommand(
            createUserDto.Email,
            createUserDto.PasswordHash,
            createUserDto.Firstname,
            createUserDto.Lastname,
            createUserDto.ProfileType,
            createUserDto.PhoneNumber);
        
        var createAccountResult = await sender.Send(createUserCommand);

        return createAccountResult.GetHttpResult(
            UserMapper.ToDto,
            UserResponseMapper.ToGetUserResponse,
            user => $"{RouteBase}/{user.Id.Value}",
            SuccessStatusCode.Created);
    }
}
