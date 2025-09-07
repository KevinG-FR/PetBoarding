namespace PetBoarding_Application.Core.Planning.ReserverCreneaux;

using PetBoarding_Application.Core.Abstractions;

public sealed record ReserverCreneauxCommand(
    string PrestationId,
    DateTime DateDebut,
    DateTime? DateFin,
    int Quantite = 1) : ICommand<bool>;