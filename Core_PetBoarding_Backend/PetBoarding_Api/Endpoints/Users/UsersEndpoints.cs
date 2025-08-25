namespace PetBoarding_Api.Endpoints.Users;

using PetBoarding_Api.Dto.Users.Responses;

public static partial class UsersEndpoints
{
    private const string RouteBase = "api/users";

    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteBase);

        group.MapGet("", GetAllUsers)
            .WithName("GetAllUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves a list of all users in the system.")
            .RequireAuthorization()
            .Produces<GetAllUsersResponse>();

        group.MapGet("{userId}", GetUser)
            .WithName("GetUserById")
            .WithSummary("Get a user by ID")
            .WithDescription("Retrieves detailed information about a specific user.")
            .Produces<GetUserResponse>()
            .Produces(404);        

        group.MapGet("/profile", GetCurrentUserProfile)
            .WithName("GetCurrentUserProfile")
            .WithSummary("Get current user profile")
            .WithDescription("Retrieves the profile of the currently authenticated user.")
            .RequireAuthorization()
            .Produces<GetUserResponse>()
            .Produces(401);

        group.MapPost("", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user account in the system.")
            .Produces<GetUserResponse>(201)
            .Produces(400);

        group.MapPut("{userId}/profile", UpdateUserProfile)
            .WithName("UpdateUserProfile")
            .WithSummary("Update user profile")
            .WithDescription("Updates the profile information of an existing user.")
            .RequireAuthorization()
            .Produces<GetUserResponse>()
            .Produces(400)
            .Produces(404);
    }
}
