namespace PetBoarding_Application.Planning.VerifierDisponibilite;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class VerifierDisponibiliteQueryHandler : IQueryHandler<VerifierDisponibiliteQuery, VerifierDisponibiliteResult>
{
    private readonly IPlanningRepository _planningRepository;

    public VerifierDisponibiliteQueryHandler(IPlanningRepository planningRepository)
    {
        _planningRepository = planningRepository;
    }

    public async Task<Result<VerifierDisponibiliteResult>> Handle(VerifierDisponibiliteQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PrestationId, out var guidValue))
        {
            return new VerifierDisponibiliteResult(
                request.PrestationId,
                false,
                new List<CreneauDisponibleResult>(),
                "ID de prestation invalide");
        }

        var prestationId = new PrestationId(guidValue);
        var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);

        if (planning == null || !planning.IsActive)
        {
            return new VerifierDisponibiliteResult(
                request.PrestationId,
                false,
                new List<CreneauDisponibleResult>(),
                "Aucun planning actif trouvé pour cette prestation");
        }

        var dateFin = request.EndDate ?? request.StartDate;
        var quantiteDemandee = request.Quantity ?? 1;
        var creneauxDisponibles = new List<CreneauDisponibleResult>();
        var isAvailable = true;

        var dateCourante = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day);
        var dateFinale = new DateTime(dateFin.Year, dateFin.Month, dateFin.Day);
        
        while (dateCourante <= dateFinale)
        {
            var creneau = planning.GetSlotForDate(dateCourante);

            if (creneau == null || creneau.AvailableCapacity < quantiteDemandee)
            {
                isAvailable = false;
                if (creneau != null)
                {
                    creneauxDisponibles.Add(new CreneauDisponibleResult(
                        creneau.Date,
                        creneau.MaxCapacity,
                        creneau.CapaciteReservee,
                        creneau.AvailableCapacity));
                }
            }
            else
            {
                creneauxDisponibles.Add(new CreneauDisponibleResult(
                    creneau.Date,
                    creneau.MaxCapacity,
                    creneau.CapaciteReservee,
                    creneau.AvailableCapacity));
            }

            dateCourante = dateCourante.AddDays(1);
        }

        var result = new VerifierDisponibiliteResult(
            request.PrestationId,
            isAvailable,
            creneauxDisponibles,
            isAvailable
                ? "Créneaux disponibles pour la période demandée"
                : "Capacité insuffisante pour certaines dates");

        return Result.Ok(result);
    }
}