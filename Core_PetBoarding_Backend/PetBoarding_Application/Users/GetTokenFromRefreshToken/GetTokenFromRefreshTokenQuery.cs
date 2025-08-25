using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Users.GetUserByEmail
{
    public record GetTokenFromRefreshTokenQuery : IQuery<string>
    {
        public string RefreshToken { get; }

        public GetTokenFromRefreshTokenQuery(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
