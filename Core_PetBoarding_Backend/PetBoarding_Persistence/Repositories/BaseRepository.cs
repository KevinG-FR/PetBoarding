using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Persistence.Repositories
{
    public abstract class BaseRepository<TEntity, TEntityIdentifier> : IBaseRepository<TEntity, TEntityIdentifier>
        where TEntity : Entity<TEntityIdentifier>
        where TEntityIdentifier : EntityIdentifier
    {
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(TEntityIdentifier entityIdentifier, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == entityIdentifier, cancellationToken);

            return entity;
        }

        public async Task<TEntity?> GetWithFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(predicate);

            return entity;
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _dbSet.ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<List<TEntity>> GetAllWithFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var newEntity = await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync();

            return newEntity.Entity;
        }

        public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var entityToUpdate = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (entityToUpdate is null)
                return null;

            entityToUpdate = entity;
            _context.SaveChanges();

            return entityToUpdate;
        }

        public async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var entityToDelete = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (entityToDelete is null)
                return 0;

            _dbSet.Remove(entityToDelete);
            return _context.SaveChanges();
        }
    }
}
