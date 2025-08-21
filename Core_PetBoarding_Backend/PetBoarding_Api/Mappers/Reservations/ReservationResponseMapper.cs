using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Api.Dto.Reservations.Responses;

namespace PetBoarding_Api.Mappers.Reservations
{
    public static class ReservationResponseMapper
    {
        public static GetReservationResponse ToGetReservationResponse(ReservationDto reservation)
        {
            return new GetReservationResponse(reservation);
        }

        public static GetAllReservationsResponse ToGetAllReservationsResponse(IReadOnlyList<ReservationDto> reservations)
        {
            return new GetAllReservationsResponse(
                reservations,
                reservations.Count);
        }
    }
}
