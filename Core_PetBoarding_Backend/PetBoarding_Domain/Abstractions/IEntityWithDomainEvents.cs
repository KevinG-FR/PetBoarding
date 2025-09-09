namespace PetBoarding_Domain.Abstractions
{
    public interface IEntityWithDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
        IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    }
}