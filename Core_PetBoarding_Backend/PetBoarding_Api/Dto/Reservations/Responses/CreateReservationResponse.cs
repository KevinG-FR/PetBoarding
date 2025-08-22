namespace PetBoarding_Api.Dto.Reservations.Responses
{
    public record CreateReservationResponse
    {
        public ReservationDto Reservation { get; init; } = new();

        public CreateReservationResponse(ReservationDto reservation)
        {
            Reservation = reservation;
        }
    }
}
