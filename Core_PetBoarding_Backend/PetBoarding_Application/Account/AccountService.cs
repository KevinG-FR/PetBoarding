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

        /// <summary>
        /// Génère un hash sécurisé du mot de passe avec SHA512 + Salt
        /// TODO: Migrer vers bcrypt/Argon2 pour plus de sécurité
        /// </summary>
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

        public async Task<string?> Authenticate(AuthenticationRequest authentificationRequest, CancellationToken cancellationToken)
        {
            Console.WriteLine($"🔍 Authenticating user: {authentificationRequest.Email}");
            
            // Récupérer l'utilisateur par email
            var emailResult = PetBoarding_Domain.Users.Email.Create(authentificationRequest.Email);
            if (emailResult.IsFailed) 
            {
                Console.WriteLine($"❌ Invalid email format: {authentificationRequest.Email}");
                return null;
            }
            
            var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (user is null) 
            {
                Console.WriteLine($"❌ User not found: {authentificationRequest.Email}");
                return null;
            }
            
            Console.WriteLine($"✅ User found: {user.Id.Value}");

            // Vérifier le mot de passe (authentificationRequest.PasswordHash contient le mot de passe en clair)
            var passwordValid = VerifyPassword(authentificationRequest.PasswordHash, user.PasswordHash);
            Console.WriteLine($"🔐 Password verification: {(passwordValid ? "SUCCESS" : "FAILED")}");
            
            if (!passwordValid)
                return null;

            var token = _jwtProvider.Generate(user);
            Console.WriteLine($"🎟️ JWT Token generated for user: {user.Id.Value}");
            return token;
        }
    }
}