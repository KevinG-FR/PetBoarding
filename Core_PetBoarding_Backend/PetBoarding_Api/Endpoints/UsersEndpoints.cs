using MediatR;

using Microsoft.AspNetCore.Mvc;

using PetBoarding_Api.Dto.Users;
using PetBoarding_Api.Extensions;

using PetBoarding_Application.Account.CreateAccount;
using PetBoarding_Application.Users.GetAllUsers;
using PetBoarding_Application.Users.GetUserById;
using PetBoarding_Application.Users.GetUserByEmail;
using PetBoarding_Application.Users.UpdateUserProfile;

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
            group.MapPost("/login", Authentification);
            group.MapPost("", CreateUser);
            group.MapPut("{userId}/profile", UpdateUserProfile).RequireAuthorization();
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
            [FromBody] LoginRequestDto loginRequest,
            ISender sender,
            IAccountService accountService)
        {
            // Utiliser la méthode Authenticate qui gère correctement la vérification du mot de passe
            var authRequest = new AuthenticationRequest(loginRequest.Email, loginRequest.Password);
            
            var token = await accountService.Authenticate(authRequest, CancellationToken.None);

            if (!string.IsNullOrEmpty(token))
            {
                // Récupérer les détails de l'utilisateur
                var userResult = await sender.Send(new GetUserByEmailQuery(loginRequest.Email));
                
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
                        Token = token,
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

        private static async Task<IResult> UpdateUserProfile(
            Guid userId,
            [FromBody] UpdateUserProfileDto updateDto,
            ISender sender)
        {
            AddressData? addressData = null;
            if (updateDto.Address != null)
            {
                addressData = new AddressData(
                    updateDto.Address.StreetNumber,
                    updateDto.Address.StreetName,
                    updateDto.Address.City,
                    updateDto.Address.PostalCode,
                    updateDto.Address.Country,
                    updateDto.Address.Complement);
            }

            var command = new UpdateUserProfileCommand(
                new UserId(userId),
                updateDto.Firstname,
                updateDto.Lastname,
                updateDto.PhoneNumber,
                addressData);

            var result = await sender.Send(command);

            return result.GetHttpResult();
        }
    }
}
