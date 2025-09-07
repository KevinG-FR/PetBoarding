namespace PetBoarding_Application.Core.Prestations.UpdatePrestation;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Caching;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class UpdatePrestationCommandHandler : ICommandHandler<UpdatePrestationCommand, Prestation>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdatePrestationCommandHandler(
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<Prestation>> Handle(UpdatePrestationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(command.Id, out var prestationId))
            {
                return Result.Fail("Invalid prestation ID format");
            }

            var prestation = await _prestationRepository.GetByIdAsync(new PrestationId(prestationId), cancellationToken);

            if (prestation is null)
            {
                return Result.Fail("Prestation not found");
            }

            if (!string.IsNullOrWhiteSpace(command.Libelle))
            {
                prestation.ModifierLibelle(command.Libelle);
            }

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                prestation.ModifierDescription(command.Description);
            }

            if (command.CategorieAnimal.HasValue)
            {
                prestation.ModifierCategorieAnimal(command.CategorieAnimal.Value);
            }

            if (command.Prix.HasValue)
            {
                prestation.ModifierPrix(command.Prix.Value);
            }

            if (command.DureeEnMinutes.HasValue)
            {
                prestation.ModifierDuree(command.DureeEnMinutes.Value);
            }

            if (command.EstDisponible.HasValue)
            {
                if (command.EstDisponible.Value)
                {
                    prestation.RendreDisponible();
                }
                else
                {
                    prestation.RendreIndisponible();
                }
            }

            var updatedPrestation = await _prestationRepository.UpdateAsync(prestation, cancellationToken);
            if (updatedPrestation is null)
            {
                return Result.Fail("Error occurred while updating the prestation");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update cache
            await _cacheService.SetAsync(CacheKeys.Prestations.ById(prestationId), updatedPrestation, null, cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.Prestations.AllPrestations(), cancellationToken);

            return Result.Ok(updatedPrestation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating prestation: {ex.Message}");
        }
    }
}
