namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public interface IPlanningRepository : IBaseRepository<Planning, PlanningId>
{
    Task<Planning?> GetByPrestationIdAsync(PrestationId prestationId, CancellationToken cancellationToken = default);
    Task<List<Planning>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<List<AvailableSlot>> GetCreneauxPourMoisAsync(PrestationId prestationId, int annee, int mois, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPrestationIdAsync(PrestationId prestationId, CancellationToken cancellationToken = default);
}