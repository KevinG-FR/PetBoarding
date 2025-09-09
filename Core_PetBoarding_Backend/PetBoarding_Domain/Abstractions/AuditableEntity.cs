namespace PetBoarding_Domain.Abstractions;

public abstract class AuditableEntity<TIdentifier> : Entity<TIdentifier>, IAuditableEntity
    where TIdentifier : EntityIdentifier
{
    // Constructeur protégé pour les classes dérivées pour EF Core.
    protected AuditableEntity() : base() { }
    protected AuditableEntity(TIdentifier id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}