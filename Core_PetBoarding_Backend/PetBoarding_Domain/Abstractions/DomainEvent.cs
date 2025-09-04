namespace PetBoarding_Domain.Abstractions;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
    }

    public Guid EventId { get; private init; }
    public DateTime OccurredOn { get; private init; }
    public string EventType { get; private init; }
}