namespace PetBoarding_Domain.Abstractions
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
        void UpdateTimestamp();
    }
}
