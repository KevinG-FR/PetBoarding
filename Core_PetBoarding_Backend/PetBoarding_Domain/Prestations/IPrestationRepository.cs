namespace PetBoarding_Domain.Prestations;

using PetBoarding_Domain.Abstractions;

public interface IPrestationRepository : IBaseRepository<Prestation, PrestationId>
{
    Task<IEnumerable<Prestation>> GetAllAsync(
        TypeAnimal? categorieAnimal = null,
        bool? estDisponible = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Prestation>> GetByCategorieAnimalAsync(
        TypeAnimal categorieAnimal, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Prestation>> GetDisponiblesAsync(
        CancellationToken cancellationToken = default);
}
