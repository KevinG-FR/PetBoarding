namespace PetBoarding_Domain.Accounts
{
    public interface IAccountService
    {
        string GetHashPassword(string password);
        
        bool VerifyPassword(string password, string hash);

        Task<AuthenticateTokens?> Authenticate(AuthenticationRequest authentificationRequest, CancellationToken cancellationToken);
    }
}
