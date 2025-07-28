using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Users;

using PetBoarding_Infrastructure.Options;

namespace PetBoarding_Infrastructure.Authentication
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtTokenOptions;

        public JwtProvider(IOptions<JwtOptions> jwtTokenOptions)
        {
            _jwtTokenOptions = jwtTokenOptions.Value;
        }
        public string Generate(User user)
        {
            var claims = new Claim[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email.Value)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8?.GetBytes(_jwtTokenOptions.Key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtTokenOptions.Issuer,
                _jwtTokenOptions.Audience,
                claims,
                null,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
