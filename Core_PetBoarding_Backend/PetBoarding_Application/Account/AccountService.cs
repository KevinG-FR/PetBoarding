using System.Security.Cryptography;
using System.Text;

using PetBoarding_Application.Abstractions;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Account
{
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
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        public async Task<string?> Authenticate(AuthenticationRequest authentificationRequest, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWithForAuthentification(authentificationRequest.Email, authentificationRequest.PasswordHash, cancellationToken);
            if (user is null)
                return null;
            var token = _jwtProvider.Generate(user);

            return token;

        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }
    }
}