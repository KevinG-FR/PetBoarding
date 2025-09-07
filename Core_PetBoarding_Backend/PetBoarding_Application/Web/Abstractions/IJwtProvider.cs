using PetBoarding_Domain.Users;
using System.Security.Claims;

namespace PetBoarding_Application.Web.Abstractions
{
    public interface IJwtProvider
    {
        string Generate(User user, int? durationInMinutes = null);
        Task<bool> ValidateRefreshToken(string refreshToken);
        IEnumerable<Claim> GetClaimsFromToken(string token);
        int GetRefreshTokenExpiryMinutes();
    }
}
