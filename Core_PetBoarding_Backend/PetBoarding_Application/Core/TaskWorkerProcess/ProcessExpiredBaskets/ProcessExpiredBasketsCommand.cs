namespace PetBoarding_Application.Core.TaskWorkerProcess.ProcessExpiredBaskets;

using PetBoarding_Application.Core.Abstractions;

/// <summary>
/// Command pour traiter les paniers expirés et libérer leurs créneaux automatiquement
/// </summary>
/// <param name="ExpirationMinutes">Nombre de minutes après lesquelles un panier est considéré comme expiré (défaut: 30)</param>
public sealed record ProcessExpiredBasketsCommand(
    int ExpirationMinutes = 30) : ICommand<int>;