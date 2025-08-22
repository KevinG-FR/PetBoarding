namespace PetBoarding_Application.Prestations.DeletePrestation;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class DeletePrestationCommandHandler : ICommandHandler<DeletePrestationCommand, bool>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePrestationCommandHandler(
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork)
    {
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeletePrestationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(command.Id, out var prestationId))
            {
                return Result.Fail<bool>("Invalid prestation ID format");
            }

            var prestation = await _prestationRepository.GetByIdAsync(new PrestationId(prestationId), cancellationToken);

            if (prestation is null)
            {
                return Result.Fail<bool>("Prestation not found");
            }

            var deleteResult = await _prestationRepository.DeleteAsync(prestation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(deleteResult > 0);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Error deleting prestation: {ex.Message}");
        }
    }
}
