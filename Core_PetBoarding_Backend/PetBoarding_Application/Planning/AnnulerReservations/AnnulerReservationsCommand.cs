namespace PetBoarding_Application.Planning.AnnulerReservations;

using PetBoarding_Application.Abstractions;

public sealed record AnnulerReservationsCommand(
    string PrestationId,
    DateTime DateDebut,
    DateTime? DateFin,
    int Quantite = 1) : ICommand<bool>;