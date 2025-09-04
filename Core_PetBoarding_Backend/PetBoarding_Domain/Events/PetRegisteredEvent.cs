using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Events;

public sealed class PetRegisteredEvent : DomainEvent
{
    public PetRegisteredEvent(PetId petId, UserId ownerId, string name, string breed, int age, decimal weight)
    {
        PetId = petId;
        OwnerId = ownerId;
        Name = name;
        Breed = breed;
        Age = age;
        Weight = weight;
    }

    public PetId PetId { get; private init; }
    public UserId OwnerId { get; private init; }
    public string Name { get; private init; }
    public string Breed { get; private init; }
    public int Age { get; private init; }
    public decimal Weight { get; private init; }
}