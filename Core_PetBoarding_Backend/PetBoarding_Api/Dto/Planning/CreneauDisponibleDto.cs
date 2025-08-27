namespace PetBoarding_Api.Dto.Planning;

/// <summary>
/// DTO représentant un créneau disponible
/// </summary>
public sealed record CreneauDisponibleDto
{
    public DateTime Date { get; init; }
    public int CapaciteMax { get; init; }
    public int CapaciteReservee { get; init; }
    public int CapaciteDisponible { get; init; }
}