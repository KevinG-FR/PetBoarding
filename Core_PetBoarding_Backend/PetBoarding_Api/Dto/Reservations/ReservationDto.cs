namespace PetBoarding_Api.Dto.Reservations
{
    public record ReservationDto
    {
        public Guid Id { get; init; }
        public string UserId { get; init; } = string.Empty;
        public string AnimalId { get; init; } = string.Empty;
        public string AnimalName { get; init; } = string.Empty;
        public string ServiceId { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string? Comments { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public decimal? TotalPrice { get; init; }
    }
}
