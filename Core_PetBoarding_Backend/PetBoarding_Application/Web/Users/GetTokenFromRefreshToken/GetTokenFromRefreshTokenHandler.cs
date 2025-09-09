using FluentResults;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Web.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Web.Users.GetTokenFromRefreshToken;

public class GetTokenFromRefreshTokenHandler : IQueryHandler<GetTokenFromRefreshTokenQuery, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public GetTokenFromRefreshTokenHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> Handle(GetTokenFromRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request?.RefreshToken))
        {
            var tokenValideted = await _jwtProvider.ValidateRefreshToken(request.RefreshToken);

            if (tokenValideted)
            {
                var claims = _jwtProvider.GetClaimsFromToken(request.RefreshToken);

                var user = await _userRepository.GetByIdAsync(
                    new UserId(Guid.Parse(claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty)));

                if (user is null)
                {
                    return Result.Fail("User not found");
                }

                var newToken = _jwtProvider.Generate(user);

                return Result.Ok(newToken);
            }
        }

        return Result.Fail("Invalid refresh token");
    }
}

