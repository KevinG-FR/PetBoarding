namespace PetBoarding_Api.Dto.Reservations.Responses
{
    public record GetReservationResponse
    {
        public ReservationDto Reservation { get; init; } = new();

        public GetReservationResponse(ReservationDto reservation)
        {
            Reservation = reservation;
        }
    }
}
