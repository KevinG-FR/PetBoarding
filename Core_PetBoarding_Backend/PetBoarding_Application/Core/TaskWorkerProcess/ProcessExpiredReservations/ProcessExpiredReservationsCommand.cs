namespace PetBoarding_Application.Core.TaskWorkerProcess.ProcessExpiredReservations;

using PetBoarding_Application.Core.Abstractions;

/// <summary>
/// Commande pour traiter les réservations expirées (non payées après 20 minutes)
/// </summary>
public sealed record ProcessExpiredReservationsCommand : ICommand<int>;