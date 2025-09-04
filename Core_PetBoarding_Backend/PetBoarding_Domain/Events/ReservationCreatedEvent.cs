using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Prestations;

namespace PetBoarding_Domain.Events;

public sealed class ReservationCreatedEvent : DomainEvent
{
    public ReservationCreatedEvent(ReservationId reservationId, UserId userId, PetId petId, PrestationId prestationId, 
        DateTime startDate, DateTime endDate, decimal totalAmount, string status)
    {
        ReservationId = reservationId;
        UserId = userId;
        PetId = petId;
        PrestationId = prestationId;
        StartDate = startDate;
        EndDate = endDate;
        TotalAmount = totalAmount;
        Status = status;
    }

    public ReservationId ReservationId { get; private init; }
    public UserId UserId { get; private init; }
    public PetId PetId { get; private init; }
    public PrestationId PrestationId { get; private init; }
    public DateTime StartDate { get; private init; }
    public DateTime EndDate { get; private init; }
    public decimal TotalAmount { get; private init; }
    public string Status { get; private init; }
}