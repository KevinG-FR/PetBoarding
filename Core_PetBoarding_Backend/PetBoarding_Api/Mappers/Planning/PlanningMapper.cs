namespace PetBoarding_Api.Mappers.Planning;

using PetBoarding_Api.Dto.Planning;
using PetBoarding_Domain.Planning;

public static class PlanningMapper
{
    public static PlanningDto ToDto(PetBoarding_Domain.Planning.Planning planning)
    {
        return new PlanningDto
        {
            Id = planning.Id.Value.ToString(),
            PrestationId = planning.PrestationId.Value.ToString(),
            Nom = planning.Label,
            Description = planning.Description,
            EstActif = planning.IsActive,
            DateCreation = planning.DateCreation,
            DateModification = planning.DateModification,
            Creneaux = planning.Creneaux.Select(ToDto).ToList()
        };
    }

    public static CreneauDisponibleDto ToDto(AvailableSlot creneau)
    {
        return new CreneauDisponibleDto
        {
            Date = creneau.Date,
            CapaciteMax = creneau.MaxCapacity,
            CapaciteReservee = creneau.CapaciteReservee,
            CapaciteDisponible = creneau.AvailableCapacity
        };
    }
}