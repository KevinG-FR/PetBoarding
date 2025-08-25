using PetBoarding_Domain.Users;
using System.Security.Claims;

namespace PetBoarding_Application.Abstractions
{
    public interface IJwtProvider
    {
        string Generate(User user, int durationInMinutes);
        Task<bool> ValidateRefreshToken(string refreshToken);
        IEnumerable<Claim> GetClaimsFromToken(string token);
    }
}
