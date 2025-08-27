namespace PetBoarding_Application.Planning.GetPlanningByPrestationId;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class GetPlanningByPrestationIdQueryHandler : IQueryHandler<GetPlanningByPrestationIdQuery, Planning>
{
    private readonly IPlanningRepository _planningRepository;

    public GetPlanningByPrestationIdQueryHandler(IPlanningRepository planningRepository)
    {
        _planningRepository = planningRepository;
    }

    public async Task<Result<Planning>> Handle(GetPlanningByPrestationIdQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PrestationId, out var guidValue))
        {
            return null;
        }

        var prestationId = new PrestationId(guidValue);
        var planningForPRestation = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);

        return Result.Ok(planningForPRestation);
    }
}