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

        public string Generate(User user, int? durationInMinutes = null)
        {
            var claims = new Claim[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email.Value)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8?.GetBytes(_jwtTokenOptions?.Key ?? throw new Exception("Secret key for JWT is null"))),
                SecurityAlgorithms.HmacSha256);

            // Utiliser la durée fournie ou la durée par défaut du token d'accès
            var expiryMinutes = durationInMinutes ?? _jwtTokenOptions.AccessTokenExpiryMinutes;
            
            var token = new JwtSecurityToken(
                _jwtTokenOptions.Issuer,
                _jwtTokenOptions.Audience,
                claims,
                null,
                DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }        

        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8?.GetBytes(_jwtTokenOptions?.Key ?? throw new Exception("Secret key for JWT is null"))),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtTokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtTokenOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var tokanValidationResult = await tokenHandler.ValidateTokenAsync(refreshToken, validationParameters);
                return tokanValidationResult.IsValid;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }

        public int GetRefreshTokenExpiryMinutes()
        {
            return _jwtTokenOptions.RefreshTokenExpiryMinutes;
        }

        public async Task<string> GenerateNewToken(string refreshToken)
        {
            if (! await ValidateRefreshToken(refreshToken))
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var claims = GetClaimsFromToken(refreshToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_jwtTokenOptions?.Key ?? throw new Exception("Secret key for JWT is null"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, "user")
                ]),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8?.GetBytes(_jwtTokenOptions?.Key ?? throw new Exception("Secret key for JWT is null"))),
                SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
