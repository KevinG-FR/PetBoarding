using PetBoarding_Api.Dto.Login.Responses;

namespace PetBoarding_Api.Endpoints.Authentication;

public static partial class AuthenticationEndpoints
{
    private const string RouteBase = "api/auth";

    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase);

        group.MapPost("/login", Authentication)
            .WithName("Login")
            .WithSummary("User authentication")
            .WithDescription("Authenticates a user and returns a JWT token.")
            .Produces<LoginResponseDto>()
            .Produces(401);

        group.MapPost("/refreshToken", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("RefreshToken")
            .WithDescription("Refresh token")
            .Produces<RefreshTokenResponseDto>()
            .Produces(401);
    }
}
