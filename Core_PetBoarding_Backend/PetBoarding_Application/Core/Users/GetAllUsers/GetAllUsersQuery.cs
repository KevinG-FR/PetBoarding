using PetBoarding_Application.Core.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetAllUsers
{
    public record GetAllUsersQuery : IQuery<List<User>> { }
}
