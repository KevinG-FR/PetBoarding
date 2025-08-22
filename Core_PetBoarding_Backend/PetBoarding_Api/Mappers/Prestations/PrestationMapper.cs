namespace PetBoarding_Api.Mappers.Prestations;

using PetBoarding_Api.Dto.Prestations;
using PetBoarding_Domain.Prestations;

public static class PrestationMapper
{
    public static PrestationDto ToDto(Prestation prestation)
    {
        return new PrestationDto
        {
            Id = prestation.Id.Value,
            Libelle = prestation.Libelle,
            Description = prestation.Description,
            CategorieAnimal = prestation.CategorieAnimal,
            Prix = prestation.Prix,
            DureeEnMinutes = prestation.DureeEnMinutes,
            EstDisponible = prestation.EstDisponible,
            DateCreation = prestation.DateCreation,
            DateModification = prestation.DateModification
        };
    }
}
