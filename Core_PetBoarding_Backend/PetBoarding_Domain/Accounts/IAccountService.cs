namespace PetBoarding_Domain.Accounts
{
    public interface IAccountService
    {
        string GetHashPassword(string password);
        
        bool VerifyPassword(string password, string hash);

        Task<string?> Authenticate(AuthenticationRequest model, CancellationToken cancellationToken);
    }
}
