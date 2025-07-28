using System.Linq.Expressions;

namespace PetBoarding_Domain.Abstractions
{
    public interface IBaseRepository<TEntity, TIdentifier> where TEntity : Entity<TIdentifier> where TIdentifier : EntityIdentifier
    {
        Task<TEntity?> GetByIdAsync(TIdentifier entityIdentifier, CancellationToken cancellationToken = default);
        Task<TEntity?> GetWithFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllWithFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}