namespace PetBoarding_Api.Dto.Users.Responses
{
    public record GetAllUsersResponse
    {
        public IReadOnlyList<UserDto> Users { get; init; } = [];
        public int TotalCount { get; init; }
        public GetAllUsersResponse(IReadOnlyList<UserDto> users, int totalCount)
        {
            Users = users;
            TotalCount = totalCount;
        }
    }
}
