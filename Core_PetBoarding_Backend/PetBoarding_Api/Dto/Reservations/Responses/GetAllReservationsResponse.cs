namespace PetBoarding_Api.Dto.Reservations.Responses
{
    public record GetAllReservationsResponse
    {
        public IReadOnlyList<ReservationDto> Reservations { get; init; } = [];
        public int TotalCount { get; init; }

        public GetAllReservationsResponse(IReadOnlyList<ReservationDto> reservations, int totalCount)
        {
            Reservations = reservations;
            TotalCount = totalCount;
        }
    }
}
