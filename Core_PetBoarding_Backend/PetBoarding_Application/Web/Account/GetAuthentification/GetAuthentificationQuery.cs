using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Accounts;

namespace PetBoarding_Application.Web.Account.GetAuthentification
{
    public record GetAuthentificationQuery : IQuery<AuthenticateTokens>
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool RememberMe { get; set; }

        public GetAuthentificationQuery(string email, string passwordHash, bool rememberMe)
        {
            Email = email;
            PasswordHash = passwordHash;
            RememberMe = rememberMe;
        }
    }
}
