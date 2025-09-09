using Newtonsoft.Json;

namespace PetBoarding_Domain.Abstractions;

public abstract class EntityIdentifier : IEquatable<EntityIdentifier>
{
    public Guid Value { get; private set; }

    [JsonConstructor]
    protected EntityIdentifier(Guid value)
    {
        Value = value;
    }
    
    // Constructeur parameterless pour la désérialisation JSON
    protected EntityIdentifier()
    {
        Value = Guid.Empty;
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityIdentifier other && Equals(other);
    }

    public bool Equals(EntityIdentifier? other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(EntityIdentifier? left, EntityIdentifier? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EntityIdentifier? left, EntityIdentifier? right)
    {
        return !Equals(left, right);
    }
}