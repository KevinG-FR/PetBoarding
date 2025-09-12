using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Repositories;

public class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override Task<User?> GetByIdAsync(UserId entityIdentifier, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == entityIdentifier, cancellationToken);
    }

    public async Task<bool> UserEmailAlreadyUsed(string email, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(email);
        if (!emailResult.IsSuccess) return false;
        
        var emailAlreadyUsed = await _dbSet.AnyAsync(x => x.Email == emailResult.Value, cancellationToken);

        return emailAlreadyUsed;
    }

    public async Task<User?> GetUserForAuthentification(string email, string passwordHash, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(email);
        if (!emailResult.IsSuccess) return null;
        
        var emailDomain = emailResult.Value;
        var user = await _dbSet
                    .Include(x => x.Address)
                    .Where(x => x.Email == emailDomain)
                    .Where(x => x.PasswordHash == passwordHash)
                    .FirstOrDefaultAsync(cancellationToken);

        return user;
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var user = await _dbSet
                    .Include(x => x.Address)
                    .Where(x => x.Email == email)
                    .FirstOrDefaultAsync(cancellationToken);

        return user;
    }

    public async Task<IEnumerable<User>> GetByIdsAsync(List<string> userIds, CancellationToken cancellationToken)
    {
        var userIdObjects = userIds.Select(id => new UserId(Guid.Parse(id))).ToList();
        
        return await _dbSet
            .Include(x => x.Address)
            .Where(x => userIdObjects.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
