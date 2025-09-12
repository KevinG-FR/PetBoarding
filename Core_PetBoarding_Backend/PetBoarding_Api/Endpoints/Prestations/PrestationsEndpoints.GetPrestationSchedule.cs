using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Prestations.Requests;
using PetBoarding_Api.Dto.Prestations.Responses;
using PetBoarding_Application.Core.Prestations.GetPrestationSchedule;

namespace PetBoarding_Api.Endpoints.Prestations;

public static partial class PrestationsEndpoints
{
    public static async Task<IResult> GetPrestationSchedule(
        [FromServices] IMediator mediator,
        [FromRoute] string id,
        [FromQuery] int year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetPrestationScheduleQuery(id, year, month);
            var result = await mediator.Send(query, cancellationToken);
            
            // Mapper le résultat Application vers DTO API
            var response = new GetPrestationScheduleResponse
            {
                PrestationId = result.PrestationId,
                PrestationName = result.PrestationName,
                Year = result.Year,
                Month = result.Month,
                ScheduleDays = result.ScheduleDays.Select(day => new ScheduleDayDto
                {
                    Date = day.Date,
                    TotalReservations = day.TotalReservations,
                    Reservations = day.Reservations.Select(reservation => new ReservationSummaryDto
                    {
                        ReservationId = reservation.ReservationId,
                        AnimalName = reservation.AnimalName,
                        UserName = reservation.UserName,
                        Status = reservation.Status,
                        StartDate = reservation.StartDate,
                        EndDate = reservation.EndDate,
                        TotalPrice = reservation.TotalPrice
                    }).ToList()
                }).ToList(),
                Statistics = new ScheduleStatistics
                {
                    TotalReservations = result.Statistics.TotalReservations,
                    ValidatedReservations = result.Statistics.ValidatedReservations,
                    InProgressReservations = result.Statistics.InProgressReservations,
                    CompletedReservations = result.Statistics.CompletedReservations,
                    CancelledReservations = result.Statistics.CancelledReservations,
                    TotalRevenue = result.Statistics.TotalRevenue,
                    BusiestDay = result.Statistics.BusiestDay,
                    MaxReservationsPerDay = result.Statistics.MaxReservationsPerDay
                }
            };
            
            return Results.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Message = "Une erreur s'est produite lors de la récupération du planning.", Details = ex.Message });
        }
    }
}