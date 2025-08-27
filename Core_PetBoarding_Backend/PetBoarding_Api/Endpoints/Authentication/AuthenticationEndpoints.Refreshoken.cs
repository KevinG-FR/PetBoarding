using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Login.Requests;
using PetBoarding_Api.Dto.Login.Responses;
using PetBoarding_Api.Extensions;
using PetBoarding_Application.Users.GetTokenFromRefreshToken;
using PetBoarding_Domain.Accounts;

namespace PetBoarding_Api.Endpoints.Authentication;

public static partial class AuthenticationEndpoints
{
    private static async Task<IResult> RefreshToken(
    [FromBody] RefreshTokenRequestDto refreshTokenRequestDto,
    ISender sender,
    IAccountService accountService)
    {
        var query = new GetTokenFromRefreshTokenQuery(refreshTokenRequestDto.RefreshToken);
        var result = await sender.Send(query);
        
        return result.GetHttpResult(
            token => token,
            token => new RefreshTokenResponseDto
            {
                Success = true,
                Token = token,
                Message = "Token refreshed successfully"
            }
        );
    }
}
