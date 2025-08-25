namespace PetBoarding_Domain.Accounts;

public class AuthenticateTokens
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;

    public AuthenticateTokens(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
}