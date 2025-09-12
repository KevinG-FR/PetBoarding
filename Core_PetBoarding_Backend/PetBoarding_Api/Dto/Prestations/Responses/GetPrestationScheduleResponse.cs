namespace PetBoarding_Api.Dto.Prestations.Responses;

public record GetPrestationScheduleResponse
{
    public string PrestationId { get; init; } = string.Empty;
    public string PrestationName { get; init; } = string.Empty;
    public int Year { get; init; }
    public int? Month { get; init; }
    public List<ScheduleDayDto> ScheduleDays { get; init; } = new();
    public ScheduleStatistics Statistics { get; init; } = new();
}

public record ScheduleDayDto
{
    public DateTime Date { get; init; }
    public int TotalReservations { get; init; }
    public List<ReservationSummaryDto> Reservations { get; init; } = new();
}

public record ReservationSummaryDto
{
    public string ReservationId { get; init; } = string.Empty;
    public string AnimalName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal? TotalPrice { get; init; }
}

public record ScheduleStatistics
{
    public int TotalReservations { get; init; }
    public int ValidatedReservations { get; init; }
    public int InProgressReservations { get; init; }
    public int CompletedReservations { get; init; }
    public int CancelledReservations { get; init; }
    public decimal TotalRevenue { get; init; }
    public DateTime? BusiestDay { get; init; }
    public int MaxReservationsPerDay { get; init; }
}