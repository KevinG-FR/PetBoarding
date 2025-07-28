using MediatR;

using Microsoft.AspNetCore.Mvc;

using PetBoarding_Api.Extensions;

using PetBoarding_Application.Account.CreateAccount;
using PetBoarding_Application.Account.GetAuthentification;
using PetBoarding_Application.Users.GetAllUsers;
using PetBoarding_Application.Users.GetUserById;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Users;

using PetBoarding_Infrastructure.Authentication;

namespace PetBoarding_Api.Endpoints
{
    public static class UsersEndpoints
    {
        public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/users");

            group.MapGet("", GetAllUsers).RequireAuthorization();
            group.MapGet("{userId}", GetUser);
            group.MapPost("/authentification", Authentification);
            group.MapPost("", CreateUser);
        }

        [HasPermission(PetBoarding_Domain.Accounts.Permission.ReadMember)]
        private static async Task<IResult> GetAllUsers(ISender sender)
        {
            var getAllUsersResult = await sender.Send(new GetAllUsersQuery());

            return getAllUsersResult.GetHttpResult();
        }

        private static async Task<IResult> GetUser(
           Guid userId,
           ISender sender)
        {
            var userResult = await sender.Send(new GetUserByIdQuery(new UserId(userId)));

            return userResult.GetHttpResult();
        }

        private static async Task<IResult> CreateUser(
            [FromBody] CreateAccountCommand createAccountCommand,
            ISender sender)
        {
            var createAccountResult = await sender.Send(createAccountCommand);

            return createAccountResult.GetHttpResult();
        }

        private static async Task<IResult> Authentification(
            [FromBody] AuthenticationRequest authenticationRequest,
            ISender sender)
        {
            var authentificationResult = await sender.Send(new GetAuthentificationQuery(authenticationRequest.Email, authenticationRequest.PasswordHash));

            return authentificationResult.GetHttpResult();
        }
    }
}
