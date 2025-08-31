namespace PetBoarding_Application.Baskets.AddItemToBasket;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

internal sealed class AddItemToBasketCommandHandler : ICommandHandler<AddItemToBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IReservationRepository _reservationRepository;

    public AddItemToBasketCommandHandler(
        IBasketRepository basketRepository,
        IReservationRepository reservationRepository)
    {
        _basketRepository = basketRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var reservationId = new ReservationId(request.ReservationId);

        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation is null)
            return Result.Fail("Reservation not found");

        if (reservation.UserId != request.UserId.ToString())
            return Result.Fail("Reservation does not belong to the user");

        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(userId, cancellationToken);
        
        if (basket is null)
        {
            basket = new Basket(userId);
            basket = await _basketRepository.AddAsync(basket, cancellationToken);
        }

        // Validation métier : vérifier s'il existe déjà une réservation similaire dans le panier
        var conflictResult = await CheckForConflictingReservation(basket, reservation, cancellationToken);
        if (conflictResult.IsFailed)
            return conflictResult;

        var addReservationResult = basket.AddReservation(reservationId);
        if (addReservationResult.IsFailed)
            return addReservationResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        return Result.Ok();
    }

    private async Task<Result> CheckForConflictingReservation(
        Basket basket, 
        Reservation newReservation, 
        CancellationToken cancellationToken)
    {
        // Si le panier est vide, pas de conflit possible
        if (basket.IsEmpty())
            return Result.Ok();

        // Récupérer toutes les réservations déjà dans le panier
        var reservationIds = basket.GetReservationIds().ToList();       

        // Récupérer les détails de toutes les réservations du panier
        var existingReservations = new List<Reservation>();
        
        foreach (var reservationId in reservationIds)
        {
            var existingReservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (existingReservation is not null)
            {
                existingReservations.Add(existingReservation);
            }
        }

        // Vérifier les conflits avec la nouvelle réservation
        foreach (var existingReservation in existingReservations)
        {
            if (HasConflict(newReservation, existingReservation))
            {
                return Result.Fail("Une réservation similaire (même service, même animal, dates qui se chevauchent) existe déjà dans le panier");
            }
        }

        return Result.Ok();
    }

    private static bool HasConflict(Reservation newReservation, Reservation existingReservation)
    {
        // Même service ET même animal
        if (newReservation.ServiceId == existingReservation.ServiceId && 
            newReservation.AnimalId == existingReservation.AnimalId)
        {
            // Vérifier si les dates se chevauchent
            return DatesOverlap(newReservation, existingReservation);
        }

        return false;
    }

    private static bool DatesOverlap(Reservation reservation1, Reservation reservation2)
    {
        var start1 = reservation1.StartDate;
        var end1 = reservation1.EndDate ?? reservation1.StartDate;
        
        var start2 = reservation2.StartDate;
        var end2 = reservation2.EndDate ?? reservation2.StartDate;

        // Deux périodes se chevauchent si :
        // - Le début de la première est avant ou égal à la fin de la seconde ET
        // - Le début de la seconde est avant ou égal à la fin de la première
        return start1 <= end2 && start2 <= end1;
    }
}