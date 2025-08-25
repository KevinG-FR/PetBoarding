using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Login.Requests;
using PetBoarding_Application.Users.GetUserByEmail;
using PetBoarding_Domain.Accounts;

namespace PetBoarding_Api.Endpoints.Authentication
{
    public static partial class AuthenticationEndpoints
    {
        private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto refreshTokenRequestDto,
        ISender sender,
        IAccountService accountService)
        {
            var query = new GetTokenFromRefreshTokenQuery(refreshTokenRequestDto.RefreshToken);
            var result = await sender.Send(query);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        }
    }
}
