using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetBoarding_Domain.Abstractions;

public abstract class AuditableEntityWithDomainEvents<TIdentifier> : EntityWithDomainEvents<TIdentifier>, IAuditableEntity
    where TIdentifier : EntityIdentifier
{
    protected AuditableEntityWithDomainEvents(TIdentifier id) : base(id)
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
