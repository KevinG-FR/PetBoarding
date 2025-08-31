namespace PetBoarding_Api.Endpoints.Baskets;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Baskets.GetUserBasket;
using PetBoarding_Api.Dto.Baskets;
using PetBoarding_Domain.Prestations;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> GetMyBasket(
        ClaimsPrincipal user,
        ISender sender,
        IPrestationRepository prestationRepository,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetUserBasketQuery(userId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        if (result.Value is null)
        {
            return Results.NoContent();
        }

        var basket = result.Value;
        
        // Load prestation details for proper pricing and service names
        var basketItemsWithPrestations = new List<BasketItemResponse>();
        foreach (var item in basket.Items)
        {
            var serviceName = "Unknown Service";
            var price = 0m;
            
            if (item.Reservation is not null && Guid.TryParse(item.Reservation.ServiceId, out var serviceGuid))
            {
                var prestationId = new PrestationId(serviceGuid);
                var prestation = await prestationRepository.GetByIdAsync(prestationId, cancellationToken);
                
                if (prestation is not null)
                {
                    serviceName = prestation.Libelle;
                    price = prestation.Prix * (item.Reservation.GetNumberOfDays());
                }
            }
            
            // Format reservation dates as string
            var reservationDatesDisplay = "Dates inconnues";
            if (item.Reservation is not null)
            {
                var startDate = item.Reservation.StartDate;
                var endDate = item.Reservation.EndDate;
                
                if (endDate.HasValue && endDate.Value.Date != startDate.Date)
                {
                    reservationDatesDisplay = $"du {startDate.Day:00}/{startDate.Month:00}/{startDate.Year} au {endDate.Value.Day:00}/{endDate.Value.Month:00}/{endDate.Value.Year}";
                }
                else
                {
                    reservationDatesDisplay = $"{startDate.Day:00}/{startDate.Month:00}/{startDate.Year}";
                }
            }
            
            basketItemsWithPrestations.Add(new BasketItemResponse(
                item.Id.Value,
                item.ReservationId.Value,
                $"{serviceName} - {item.Reservation?.AnimalName ?? "Unknown"}",
                price,
                reservationDatesDisplay
            ));
        }
        
        var basketResponse = new BasketResponse(
            basket.Id.Value,
            basket.UserId.Value,
            basket.Status.Name,
            basketItemsWithPrestations.Sum(item => item.ReservationPrice),
            basket.GetTotalItemCount(),
            basket.PaymentFailureCount,
            basket.PaymentId?.Value,
            basket.CreatedAt,
            basket.UpdatedAt,
            basketItemsWithPrestations
        );

        return Results.Ok(basketResponse);
    }
}