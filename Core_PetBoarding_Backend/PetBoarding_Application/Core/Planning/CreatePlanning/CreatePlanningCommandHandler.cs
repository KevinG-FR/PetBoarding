namespace PetBoarding_Application.Core.Planning.CreatePlanning;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class CreatePlanningCommandHandler : ICommandHandler<CreatePlanningCommand, string>
{
    private readonly IPlanningRepository _planningRepository;
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanningCommandHandler(
        IPlanningRepository planningRepository,
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork)
    {
        _planningRepository = planningRepository;
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(CreatePlanningCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PrestationId, out var guidValue))
        {
            throw new ArgumentException("ID de prestation invalide", nameof(request.PrestationId));
        }

        var prestationId = new PrestationId(guidValue);
        
        // Vérifier que la prestation existe
        var prestation = await _prestationRepository.GetByIdAsync(prestationId, cancellationToken);
        if (prestation is null)
        {
            throw new InvalidOperationException("Prestation introuvable");
        }

        // Vérifier qu'il n'existe pas déjà un planning pour cette prestation
        var existingPlanning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);
        if (existingPlanning is not null)
        {
            throw new InvalidOperationException("Un planning existe déjà pour cette prestation");
        }

        var planningId = PlanningId.New();
        var planning = new Planning(planningId, prestationId, request.Nom, request.Description);

        await _planningRepository.AddAsync(planning, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(planningId.Value.ToString());
    }
}