namespace PetBoarding_Domain.Accounts
{
    public class AuthenticationRequest
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public AuthenticationRequest(string email, string passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
