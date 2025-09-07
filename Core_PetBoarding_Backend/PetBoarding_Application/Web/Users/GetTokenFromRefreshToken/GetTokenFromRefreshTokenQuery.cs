using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Web.Users.GetTokenFromRefreshToken
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
