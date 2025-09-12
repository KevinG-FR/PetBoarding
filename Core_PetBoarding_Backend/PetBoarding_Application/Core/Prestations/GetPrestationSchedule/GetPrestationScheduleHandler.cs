using MediatR;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Prestations.GetPrestationSchedule.Models;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Prestations.GetPrestationSchedule;

public class GetPrestationScheduleHandler : IRequestHandler<GetPrestationScheduleQuery, PrestationScheduleResult>
{
    private readonly IPrestationRepository _prestationRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetPrestationScheduleHandler> _logger;

    public GetPrestationScheduleHandler(
        IPrestationRepository prestationRepository,
        IReservationRepository reservationRepository,
        IUserRepository userRepository,
        ILogger<GetPrestationScheduleHandler> logger)
    {
        _prestationRepository = prestationRepository;
        _reservationRepository = reservationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<PrestationScheduleResult> Handle(GetPrestationScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting schedule for prestation {PrestationId}, Year: {Year}, Month: {Month}", 
            request.PrestationId, request.Year, request.Month);

        // Vérifier que la prestation existe
        var prestation = await _prestationRepository.GetByIdAsync(new PrestationId(Guid.Parse(request.PrestationId)), cancellationToken);
        if (prestation is null)
        {
            throw new InvalidOperationException($"Prestation with ID {request.PrestationId} not found");
        }

        // Définir la période de recherche
        DateTime startDate;
        DateTime endDate;

        if (request.Month.HasValue)
        {
            // Vue mensuelle
            startDate = new DateTime(request.Year, request.Month.Value, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
        }
        else
        {
            // Vue annuelle
            startDate = new DateTime(request.Year, 1, 1);
            endDate = new DateTime(request.Year, 12, 31);
        }

        // Récupérer les réservations pour cette prestation et cette période
        var reservations = await _reservationRepository.GetByServiceIdAndDateRangeAsync(
            request.PrestationId, 
            startDate, 
            endDate, 
            cancellationToken);

        // Grouper les réservations par date
        var reservationGroups = reservations
            .SelectMany(r => r.GetReservedDates().Select(date => new { Date = date, Reservation = r }))
            .Where(x => x.Date >= startDate && x.Date <= endDate)
            .GroupBy(x => x.Date.Date)
            .ToList();

        // Récupérer les utilisateurs pour obtenir leurs noms
        var userIds = reservations.Select(r => r.UserId).Distinct().ToList();
        var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var userDictionary = users.ToDictionary(u => u.Id.Value.ToString(), u => $"{u.Firstname} {u.Lastname}");

        // Construire la liste des jours du planning
        var scheduleDays = new List<ScheduleDayResult>();
        
        // Pour la vue mensuelle, inclure tous les jours du mois
        // Pour la vue annuelle, inclure seulement les jours avec des réservations
        if (request.Month.HasValue)
        {
            // Vue mensuelle : tous les jours du mois
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                var dayReservations = reservationGroups
                    .FirstOrDefault(g => g.Key == currentDate)?
                    .Select(x => x.Reservation)
                    .Distinct()
                    .ToList() ?? new List<Reservation>();

                scheduleDays.Add(CreateScheduleDay(currentDate, dayReservations, userDictionary));
                currentDate = currentDate.AddDays(1);
            }
        }
        else
        {
            // Vue annuelle : seulement les jours avec réservations
            foreach (var group in reservationGroups.OrderBy(g => g.Key))
            {
                var dayReservations = group.Select(x => x.Reservation).Distinct().ToList();
                scheduleDays.Add(CreateScheduleDay(group.Key, dayReservations, userDictionary));
            }
        }

        // Calculer les statistiques
        var statistics = CalculateStatistics(reservations.ToList(), reservationGroups);

        return new PrestationScheduleResult
        {
            PrestationId = request.PrestationId,
            PrestationName = prestation.Libelle,
            Year = request.Year,
            Month = request.Month,
            ScheduleDays = scheduleDays,
            Statistics = statistics
        };
    }

    private static ScheduleDayResult CreateScheduleDay(DateTime date, List<Reservation> dayReservations, Dictionary<string, string> userDictionary)
    {
        var reservationSummaries = dayReservations.Select(r => new ReservationSummaryResult
        {
            ReservationId = r.Id.Value.ToString(),
            AnimalName = r.AnimalName,
            UserName = userDictionary.GetValueOrDefault(r.UserId, "Utilisateur inconnu"),
            Status = r.Status.ToString(),
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            TotalPrice = r.TotalPrice
        }).ToList();

        return new ScheduleDayResult
        {
            Date = date,
            TotalReservations = dayReservations.Count,
            Reservations = reservationSummaries
        };
    }

    private static ScheduleStatisticsResult CalculateStatistics(List<Reservation> allReservations, IEnumerable<IGrouping<DateTime, dynamic>> reservationGroups)
    {
        var totalReservations = allReservations.Count;
        var validatedCount = allReservations.Count(r => r.Status == ReservationStatus.Validated);
        var inProgressCount = allReservations.Count(r => r.Status == ReservationStatus.InProgress);
        var completedCount = allReservations.Count(r => r.Status == ReservationStatus.Completed);
        var cancelledCount = allReservations.Count(r => r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.CancelAuto);

        var totalRevenue = allReservations
            .Where(r => r.Status == ReservationStatus.Completed && r.TotalPrice.HasValue)
            .Sum(r => r.TotalPrice.Value);

        DateTime? busiestDay = null;
        var maxReservationsPerDay = 0;

        if (reservationGroups.Any())
        {
            var busiestDayGroup = reservationGroups.OrderByDescending(g => g.Count()).First();
            busiestDay = busiestDayGroup.Key;
            maxReservationsPerDay = busiestDayGroup.Count();
        }

        return new ScheduleStatisticsResult
        {
            TotalReservations = totalReservations,
            ValidatedReservations = validatedCount,
            InProgressReservations = inProgressCount,
            CompletedReservations = completedCount,
            CancelledReservations = cancelledCount,
            TotalRevenue = totalRevenue,
            BusiestDay = busiestDay,
            MaxReservationsPerDay = maxReservationsPerDay
        };
    }
}