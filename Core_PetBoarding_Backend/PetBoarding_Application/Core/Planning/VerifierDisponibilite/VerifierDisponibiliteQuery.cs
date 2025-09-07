namespace PetBoarding_Application.Core.Planning.VerifierDisponibilite;

using PetBoarding_Application.Core.Abstractions;

public sealed record VerifierDisponibiliteQuery(
    string PrestationId,
    DateTime StartDate,
    DateTime? EndDate = null,
    int? Quantity = null) : IQuery<VerifierDisponibiliteResult>;

public sealed record VerifierDisponibiliteResult(
    string PrestationId,
    bool IsAvailable,
    List<CreneauDisponibleResult> AvailableSlots,
    string? Message);

public sealed record CreneauDisponibleResult(
    DateTime Date,
    int CapaciteMax,
    int CapaciteReservee,
    int CapaciteDisponible);