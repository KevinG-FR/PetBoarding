using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Domain.Reservations;

namespace PetBoarding_Api.Mappers.Reservations
{
    public static class ReservationMapper
    {
        public static ReservationDto ToDto(Reservation reservation)
        {
            return new ReservationDto
            {
                Id = reservation.Id.Value,
                UserId = reservation.UserId,
                AnimalId = reservation.AnimalId,
                AnimalName = reservation.AnimalName,
                ServiceId = reservation.ServiceId,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                Comments = reservation.Comments,
                Status = reservation.Status.ToString(),
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt,
                TotalPrice = reservation.TotalPrice
            };
        }
    }
}
