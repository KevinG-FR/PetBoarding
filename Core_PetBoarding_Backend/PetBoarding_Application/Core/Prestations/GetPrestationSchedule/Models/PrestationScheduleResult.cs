namespace PetBoarding_Application.Core.Prestations.GetPrestationSchedule.Models;

public record PrestationScheduleResult
{
    public string PrestationId { get; init; } = string.Empty;
    public string PrestationName { get; init; } = string.Empty;
    public int Year { get; init; }
    public int? Month { get; init; }
    public List<ScheduleDayResult> ScheduleDays { get; init; } = new();
    public ScheduleStatisticsResult Statistics { get; init; } = new();
}

public record ScheduleDayResult
{
    public DateTime Date { get; init; }
    public int TotalReservations { get; init; }
    public List<ReservationSummaryResult> Reservations { get; init; } = new();
}

public record ReservationSummaryResult
{
    public string ReservationId { get; init; } = string.Empty;
    public string AnimalName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal? TotalPrice { get; init; }
}

public record ScheduleStatisticsResult
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