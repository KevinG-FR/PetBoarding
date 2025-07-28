using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetAllUsers
{
    public record GetAllUsersQuery : IQuery<List<User>> { }
}
