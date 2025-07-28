using PetBoarding_Application.Abstractions;

namespace PetBoarding_Application.Account.GetAuthentification
{
    public record GetAuthentificationQuery : IQuery<string>
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public GetAuthentificationQuery(string email, string passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
