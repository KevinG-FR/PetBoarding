using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserByEmail
{
    public record GetUserByEmailQuery : IQuery<User>
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
