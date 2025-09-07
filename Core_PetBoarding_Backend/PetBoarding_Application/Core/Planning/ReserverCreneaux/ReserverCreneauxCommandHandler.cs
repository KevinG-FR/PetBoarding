namespace PetBoarding_Application.Core.Planning.ReserverCreneaux;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class ReserverCreneauxCommandHandler : ICommandHandler<ReserverCreneauxCommand, bool>
{
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserverCreneauxCommandHandler(IPlanningRepository planningRepository, IUnitOfWork unitOfWork)
    {
        _planningRepository = planningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ReserverCreneauxCommand request, CancellationToken cancellationToken)
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

        // Vérifier d'abord la disponibilité pour toutes les dates
        var dateCourante = new DateTime(request.DateDebut.Year, request.DateDebut.Month, request.DateDebut.Day);
        var dateFinale = new DateTime(dateFinal.Year, dateFinal.Month, dateFinal.Day);
        
        while (dateCourante <= dateFinale)
        {
            if (!planning.IsAvailableForDate(dateCourante, request.Quantite))
            {
                return false;
            }
            dateCourante = dateCourante.AddDays(1);
        }

        // Réserver tous les créneaux
        dateCourante = new DateTime(request.DateDebut.Year, request.DateDebut.Month, request.DateDebut.Day);
        while (dateCourante <= dateFinale)
        {
            try
            {
                planning.ReserveSlot(dateCourante, request.Quantite);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            dateCourante = dateCourante.AddDays(1);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}