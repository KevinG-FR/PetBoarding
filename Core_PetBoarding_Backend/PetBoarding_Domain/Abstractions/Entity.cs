namespace PetBoarding_Domain.Abstractions;

public abstract class Entity<TIdentifier> : IEquatable<Entity<EntityIdentifier>> 
    where TIdentifier : EntityIdentifier
{
    private const int MULTIPLIER_GET_HASH_CODE = 42;

    // Constructeur protégé pour les classes dérivées pour EF Core.
    protected Entity() { }
    protected Entity(TIdentifier id)
    {
        Id = id;
    }

    public TIdentifier Id { get; private init; }

    public static bool operator ==(Entity<TIdentifier> left, Entity<TIdentifier> right)
    {
        return left is not null &&
                right is not null &&
                left.Equals(right);
    }

    public static bool operator !=(Entity<TIdentifier> left, Entity<TIdentifier> right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() != GetType())
            return false;

        if (obj is not Entity<TIdentifier> entity)
            return false;

        return entity.Id == Id;
    }

    public bool Equals(Entity<EntityIdentifier>? other)
    {
        if (other is null)
            return false;

        if (other.GetType() != GetType())
            return false;

        return other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * MULTIPLIER_GET_HASH_CODE;
    }
}