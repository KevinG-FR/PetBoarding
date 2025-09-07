namespace PetBoarding_Application.Core.Planning.AnnulerReservations;

using PetBoarding_Application.Core.Abstractions;

public sealed record AnnulerReservationsCommand(
    string PrestationId,
    DateTime DateDebut,
    DateTime? DateFin,
    int Quantite = 1) : ICommand<bool>;