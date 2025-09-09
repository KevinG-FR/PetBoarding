namespace PetBoarding_Application.Core.Prestations.CreatePrestation;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class CreatePrestationCommandHandler : ICommandHandler<CreatePrestationCommand, Prestation>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreatePrestationCommandHandler(
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<Prestation>> Handle(CreatePrestationCommand command, CancellationToken cancellationToken)
    {
        try
        {            
            var prestation = Prestation.Create(
                command.Libelle,
                command.Description,
                command.CategorieAnimal,
                command.Prix,
                command.DureeEnMinutes);

            await _prestationRepository.AddAsync(prestation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await _cacheService.RemoveAsync(CacheKeys.Prestations.AllPrestations(), cancellationToken);

            return Result.Ok(prestation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating prestation: {ex.Message}");
        }
    }
}
