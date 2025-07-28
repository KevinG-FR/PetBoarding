using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Repositories
{
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<bool> UserEmailAlreadyUsed(string email, CancellationToken cancellationToken)
        {
            var emailAlreadyUsed = await _dbSet.AnyAsync(x => x.Email == Email.Create(email).Value, cancellationToken);

            return emailAlreadyUsed;
        }

        public async Task<User?> GetUserWithForAuthentification(string email, string passwordHash, CancellationToken cancellationToken)
        {
            var emailDomain = Email.Create(email).Value;
            var user = await _dbSet
                        .Where(x => x.Email == emailDomain)
                        .Where(x => x.PasswordHash == passwordHash)
                        .FirstOrDefaultAsync(cancellationToken);

            return user;
        }
    }
}
