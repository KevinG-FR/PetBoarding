namespace PetBoarding_Application.Planning.ReserverCreneaux;

using PetBoarding_Application.Abstractions;

public sealed record ReserverCreneauxCommand(
    string PrestationId,
    DateTime DateDebut,
    DateTime? DateFin,
    int Quantite = 1) : ICommand<bool>;