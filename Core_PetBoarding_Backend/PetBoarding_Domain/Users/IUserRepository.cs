using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Users
{
    public interface IUserRepository : IBaseRepository<User, UserId>
    {
        Task<bool> UserEmailAlreadyUsed(string email, CancellationToken cancellationToken);

        Task<User?> GetUserWithForAuthentification(string email, string passwordHash, CancellationToken cancellationToken);
    }
}
