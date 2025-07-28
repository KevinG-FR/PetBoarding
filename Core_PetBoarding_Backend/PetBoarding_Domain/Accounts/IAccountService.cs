namespace PetBoarding_Domain.Accounts
{
    public interface IAccountService
    {
        string GetHashPassword(string password);

        Task<string?> Authenticate(AuthenticationRequest model, CancellationToken cancellationToken);
    }
}
