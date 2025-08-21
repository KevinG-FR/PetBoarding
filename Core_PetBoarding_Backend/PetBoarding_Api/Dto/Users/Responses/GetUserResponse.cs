namespace PetBoarding_Api.Dto.Users.Responses
{
    public record GetUserResponse
    {
        public UserDto User { get; init; } = new();

        public GetUserResponse(UserDto user)
        {
            User = user;
        }
    }
}
