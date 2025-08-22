namespace PetBoarding_Application.Prestations.GetPrestationById;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class GetPrestationByIdQueryHandler : IQueryHandler<GetPrestationByIdQuery, Prestation>
{
    private readonly IPrestationRepository _prestationRepository;

    public GetPrestationByIdQueryHandler(IPrestationRepository prestationRepository)
    {
        _prestationRepository = prestationRepository;
    }

    public async Task<Result<Prestation>> Handle(GetPrestationByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(query.Id, out var prestationId))
            {
                return Result.Fail("Invalid prestation ID format");
            }

            var prestation = await _prestationRepository.GetByIdAsync(new PrestationId(prestationId), cancellationToken);

            if (prestation is null)
            {
                return Result.Fail("Prestation not found");
            }

            return Result.Ok(prestation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving prestation: {ex.Message}");
        }
    }
}
