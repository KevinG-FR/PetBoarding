using PetBoarding_Api.Dto.Users.Responses;

namespace PetBoarding_Api.Mappers.Users
{
    public static class UserResponseMapper
    {
        public static GetUserResponse ToGetUserResponse(UserDto user)
        {
            return new GetUserResponse(user);
        }

        public static GetAllUsersResponse ToGetAllUsersResponse(IReadOnlyList<UserDto> users)
        {
            return new GetAllUsersResponse(
                users,
                users.Count);
        }
    }
}
