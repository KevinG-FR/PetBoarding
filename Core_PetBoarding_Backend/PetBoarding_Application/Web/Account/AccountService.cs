using System.Security.Cryptography;
using System.Text;

using PetBoarding_Application.Web.Abstractions;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Web.Account;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public AccountService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }
            
    public string GetHashPassword(string password)
    {
        // Générer un salt aléatoire
        var salt = GenerateSalt();
        
        // Combiner le mot de passe et le salt
        var passwordWithSalt = password + salt;
        
        using var sha512 = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
        var hash = sha512.ComputeHash(bytes);
        
        // Retourner salt + hash encodé en base64
        return salt + ":" + Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Vérifie si un mot de passe correspond à son hash
    /// </summary>
    public bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;
            
            var salt = parts[0];
            var hash = parts[1];
            
            // Recalculer le hash avec le même salt
            var passwordWithSalt = password + salt;
            using var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            var computedHash = sha512.ComputeHash(bytes);
            var computedHashString = Convert.ToBase64String(computedHash);
            
            return hash == computedHashString;
        }
        catch
        {
            return false;
        }
    }

    private static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    public async Task<AuthenticateTokens?> Authenticate(AuthenticationRequest authentificationRequest, CancellationToken cancellationToken)
    {
        var emailResult = PetBoarding_Domain.Users.Email.Create(authentificationRequest.Email);
        if (emailResult.IsFailed) 
        {
            return null;
        }
        
        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null) 
        {
            return null;
        }
        

        var passwordValid = VerifyPassword(authentificationRequest.PasswordHash, user.PasswordHash);
        
        if (!passwordValid)
            return null;

        var token = _jwtProvider.Generate(user);

        string refreshToken = null;
        if (authentificationRequest.RememberMe)
        {
            refreshToken = _jwtProvider.Generate(user, _jwtProvider.GetRefreshTokenExpiryMinutes());
            return new AuthenticateTokens(token, refreshToken);
        }

        if (string.IsNullOrEmpty(token))
            return null;

        return new AuthenticateTokens(token, refreshToken);
    }
}