namespace PetBoarding_Application.Reservations.Models;

using PetBoarding_Domain.Reservations;

/// <summary>
/// Modèle pour l'affichage des réservations avec les informations de prestation
/// Utilisé spécifiquement pour les vues utilisateur simplifiées
/// </summary>
public sealed class ReservationWithPrestationInfo
{
    public Guid Id { get; init; }
    public string AnimalName { get; init; } = string.Empty;
    public string PrestationLibelle { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Comments { get; init; }
    public ReservationStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal? TotalPrice { get; init; }

    public ReservationWithPrestationInfo(
        Guid id,
        string animalName,
        string prestationLibelle,
        DateTime startDate,
        DateTime? endDate,
        string? comments,
        ReservationStatus status,
        DateTime createdAt,
        decimal? totalPrice)
    {
        Id = id;
        AnimalName = animalName;
        PrestationLibelle = prestationLibelle;
        StartDate = startDate;
        EndDate = endDate;
        Comments = comments;
        Status = status;
        CreatedAt = createdAt;
        TotalPrice = totalPrice;
    }
}