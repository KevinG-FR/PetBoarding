using Newtonsoft.Json;

namespace PetBoarding_Domain.Abstractions;

public abstract class EntityWithDomainEvents<TIdentifier> : Entity<TIdentifier>, IEntityWithDomainEvents 
    where TIdentifier : EntityIdentifier
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected EntityWithDomainEvents(TIdentifier id) : base(id)
    {
    }

    [JsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
