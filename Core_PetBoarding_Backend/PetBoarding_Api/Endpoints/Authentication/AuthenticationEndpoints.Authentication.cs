using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Addresses;
using PetBoarding_Api.Dto.Login.Requests;
using PetBoarding_Api.Dto.Login.Responses;
using PetBoarding_Application.Users.GetUserByEmail;
using PetBoarding_Domain.Accounts;

namespace PetBoarding_Api.Endpoints.Authentication
{
    public static partial class AuthenticationEndpoints
    {
        private static async Task<IResult> Authentication(
        [FromBody] LoginRequestDto loginRequestDto,
        ISender sender,
        IAccountService accountService)
        {
            var authRequest = new AuthenticationRequest(loginRequestDto.Email, loginRequestDto.Password, loginRequestDto.RememberMe);

            var authenticateTokens = await accountService.Authenticate(authRequest, CancellationToken.None);

            if (!string.IsNullOrEmpty(authenticateTokens.Token))
            {
                var userResult = await sender.Send(new GetUserByEmailQuery(loginRequestDto.Email));

                if (userResult.IsSuccess)
                {
                    var user = userResult.Value;

                    AddressDto? addressDto = null;
                    if (user.Address is not null)
                    {
                        addressDto = new AddressDto(
                            user.Address.StreetNumber.Value,
                            user.Address.StreetName.Value,
                            user.Address.City.Value,
                            user.Address.PostalCode.Value,
                            user.Address.Country.Value,
                            user.Address.Complement?.Value);
                    }

                    var userDto = new UserDto
                    {
                        Id = user.Id.Value,
                        Email = user.Email.Value,
                        FirstName = user.Firstname.Value,
                        LastName = user.Lastname.Value,
                        PhoneNumber = user.PhoneNumber.Value,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        ProfileType = user.ProfileType.ToString(),
                        Status = user.Status.ToString(),
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt,
                        Address = addressDto
                    };

                    var response = new LoginResponseDto
                    {
                        Success = true,
                        Message = "Connexion réussie",
                        Token = authenticateTokens.Token,
                        RefreshToken = authenticateTokens.RefreshToken,
                        User = userDto
                    };

                    return Results.Ok(response);
                }
            }

            var errorResponse = new LoginResponseDto
            {
                Success = false,
                Message = "Email ou mot de passe incorrect"
            };

            return Results.Unauthorized();
        }
    }
}
