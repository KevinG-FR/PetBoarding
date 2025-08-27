using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Pets;

public interface IPetRepository : IBaseRepository<Pet, PetId>
{
    new Task<IEnumerable<Pet>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Pet>> GetByOwnerIdAsync(
        UserId ownerId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Pet>> GetByTypeAsync(
        PetType type, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Pet>> GetByTypeAndOwnerAsync(
        PetType type, 
        UserId ownerId, 
        CancellationToken cancellationToken = default);
    
    Task<Pet?> GetByIdWithOwnerAsync(
        PetId petId, 
        CancellationToken cancellationToken = default);
}