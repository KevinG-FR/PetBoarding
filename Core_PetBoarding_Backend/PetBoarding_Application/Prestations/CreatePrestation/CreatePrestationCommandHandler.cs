namespace PetBoarding_Application.Prestations.CreatePrestation;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class CreatePrestationCommandHandler : ICommandHandler<CreatePrestationCommand, Prestation>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePrestationCommandHandler(
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork)
    {
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Prestation>> Handle(CreatePrestationCommand command, CancellationToken cancellationToken)
    {
        try
        {            
            var prestation = new Prestation(
                command.Libelle,
                command.Description,
                command.CategorieAnimal,
                command.Prix,
                command.DureeEnMinutes);

            await _prestationRepository.AddAsync(prestation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(prestation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating prestation: {ex.Message}");
        }
    }
}
