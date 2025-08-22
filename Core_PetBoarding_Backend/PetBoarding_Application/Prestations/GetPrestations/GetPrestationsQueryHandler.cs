namespace PetBoarding_Application.Prestations.GetPrestations;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Prestations;

public sealed class GetPrestationsQueryHandler : IQueryHandler<GetPrestationsQuery, IEnumerable<Prestation>>
{
    private readonly IPrestationRepository _prestationRepository;

    public GetPrestationsQueryHandler(IPrestationRepository prestationRepository)
    {
        _prestationRepository = prestationRepository;
    }

    public async Task<Result<IEnumerable<Prestation>>> Handle(GetPrestationsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var prestations = await _prestationRepository.GetAllAsync(cancellationToken);

            var filteredPrestations = prestations.AsEnumerable();

            // Filtrage par catégorie d'animal
            if (query.CategorieAnimal.HasValue)
            {
                filteredPrestations = filteredPrestations.Where(p => p.CategorieAnimal == query.CategorieAnimal.Value);
            }

            // Filtrage par disponibilité
            if (query.EstDisponible.HasValue)
            {
                filteredPrestations = filteredPrestations.Where(p => p.EstDisponible == query.EstDisponible.Value);
            }

            // Filtrage par texte de recherche (libellé ou description)
            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var searchText = query.SearchText.ToLowerInvariant();
                filteredPrestations = filteredPrestations.Where(p => 
                    p.Libelle.ToLowerInvariant().Contains(searchText) ||
                    p.Description.ToLowerInvariant().Contains(searchText));
            }

            return Result.Ok(filteredPrestations);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving prestations: {ex.Message}");
        }
    }
}
