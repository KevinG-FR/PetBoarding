namespace PetBoarding_Application.Core.Planning.GetAllPlannings;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Planning;

internal sealed class GetAllPlanningsQueryHandler : IQueryHandler<GetAllPlanningsQuery, List<Planning>>
{
    private readonly IPlanningRepository _planningRepository;

    public GetAllPlanningsQueryHandler(IPlanningRepository planningRepository)
    {
        _planningRepository = planningRepository;
    }

    public async Task<Result<List<Planning>>> Handle(GetAllPlanningsQuery request, CancellationToken cancellationToken)
    {
        var availablePlannings =  await _planningRepository.GetAllActiveAsync(cancellationToken);

        return Result.Ok(availablePlannings);

    }
}