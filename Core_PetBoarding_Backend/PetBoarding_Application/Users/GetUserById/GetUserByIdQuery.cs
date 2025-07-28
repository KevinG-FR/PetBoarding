using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserById
{
    public record GetUserByIdQuery(UserId UserId) : IQuery<User> { }
}
