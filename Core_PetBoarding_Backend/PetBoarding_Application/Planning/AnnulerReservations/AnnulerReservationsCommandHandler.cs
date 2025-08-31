namespace PetBoarding_Application.Planning.AnnulerReservations;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class AnnulerReservationsCommandHandler : ICommandHandler<AnnulerReservationsCommand, bool>
{
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnulerReservationsCommandHandler(IPlanningRepository planningRepository, IUnitOfWork unitOfWork)
    {
        _planningRepository = planningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AnnulerReservationsCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PrestationId, out var guidValue))
        {
            return false;
        }

        var prestationId = new PrestationId(guidValue);
        var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);

        if (planning is null)
        {
            return false;
        }

        var dateFinal = request.DateFin ?? request.DateDebut;

        // Annuler les réservations pour toutes les dates
        var dateCourante = new DateTime(request.DateDebut.Year, request.DateDebut.Month, request.DateDebut.Day);
        var dateFinale = new DateTime(dateFinal.Year, dateFinal.Month, dateFinal.Day);
        
        while (dateCourante <= dateFinale)
        {
            try
            {
                planning.CancelReservation(dateCourante, request.Quantite);
            }
            catch (InvalidOperationException)
            {
                // Continue même si l'annulation d'une date échoue
                // car il se peut qu'il n'y ait pas de réservation pour cette date
            }
            dateCourante = dateCourante.AddDays(1);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(true);
    }
}