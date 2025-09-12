using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Users
{
    public interface IUserRepository : IBaseRepository<User, UserId>
    {
        Task<bool> UserEmailAlreadyUsed(string email, CancellationToken cancellationToken);

        Task<User?> GetUserForAuthentification(string email, string passwordHash, CancellationToken cancellationToken);

        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);

        Task<IEnumerable<User>> GetByIdsAsync(List<string> userIds, CancellationToken cancellationToken);
    }
}
