using FluentResults;

namespace PetBoarding_Domain.Errors.Entities
{
    public class EntityNotFoundError<TEntity> : Error, INotFoundError
    {
        public EntityNotFoundError()
            : base($"{typeof(TEntity).Name} not found.")
        {
        }

        public EntityNotFoundError(Guid id)
            : base($"{typeof(TEntity).Name} not found for id '{id}'.")
        {
        }
    }
}
