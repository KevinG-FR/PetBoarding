using PetBoarding_Api.Dto.Pets;
using PetBoarding_Api.Dto.Pets.Responses;

namespace PetBoarding_Api.Mappers.Pets;

public static class PetResponseMapper
{
    public static GetAllPetsResponse ToGetAllPetsResponse(IEnumerable<PetDto> pets)
    {
        var petsList = pets.ToList();
        return new GetAllPetsResponse
        {
            Pets = petsList,
            Count = petsList.Count,
            Success = true,
            Message = "Pets retrieved successfully"
        };
    }

    public static GetPetResponse ToGetPetResponse(PetDto pet)
    {
        return new GetPetResponse
        {
            Pet = pet,
            Success = true,
            Message = "Pet retrieved successfully"
        };
    }

    public static CreatePetResponse ToCreatePetResponse(PetDto pet)
    {
        return new CreatePetResponse
        {
            Pet = pet,
            Success = true,
            Message = "Pet created successfully",
            Location = $"/api/v1/pets/{pet.Id}"
        };
    }

    public static UpdatePetResponse ToUpdatePetResponse(PetDto pet)
    {
        return new UpdatePetResponse
        {
            Pet = pet,
            Success = true,
            Message = "Pet updated successfully"
        };
    }

    public static DeletePetResponse ToDeletePetResponse(object deletedData)
    {
        // L'objet deletedData contient { PetId = id }
        var petId = deletedData.GetType().GetProperty("PetId")?.GetValue(deletedData) as Guid?;
        
        return new DeletePetResponse
        {
            Success = true,
            Message = "Pet deleted successfully",
            PetId = petId
        };
    }
}