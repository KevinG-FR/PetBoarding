namespace PetBoarding_Domain.Accounts;

public class AuthenticationRequest
{
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public bool RememberMe { get; init; } = false;

    public AuthenticationRequest(string email, string passwordHash, bool remerberMe)
    {
        Email = email;
        PasswordHash = passwordHash;
        RememberMe = remerberMe;
    }
}
