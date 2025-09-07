using PetBoarding_Application.Core.Abstractions;

using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetUserById
{
    public record GetUserByIdQuery(UserId UserId) : IQuery<User> { }
}
