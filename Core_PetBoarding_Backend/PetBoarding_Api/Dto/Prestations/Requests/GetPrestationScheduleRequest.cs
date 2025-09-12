using System.ComponentModel.DataAnnotations;

namespace PetBoarding_Api.Dto.Prestations.Requests;

public record GetPrestationScheduleRequest
{
    [Required]
    public string PrestationId { get; init; } = string.Empty;

    [Required]
    [Range(2020, 2030)]
    public int Year { get; init; }

    [Range(1, 12)]
    public int? Month { get; init; }
}