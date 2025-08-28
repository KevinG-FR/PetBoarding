namespace PetBoarding_Api.Endpoints.Planning;

using MediatR;
using PetBoarding_Api.Dto.Planning;
using PetBoarding_Api.Dto.Planning.Responses;
using PetBoarding_Application.Planning.VerifierDisponibilite;

public static partial class PlanningEndpoints
{
    public static async Task<IResult> VerifierDisponibilite(DisponibiliteQueryDto request, ISender sender)
    {
        var query = new VerifierDisponibiliteQuery(
            request.PrestationId,
            request.StartDate,
            request.EndDate,
            request.Quantity);

        var result = await sender.Send(query);

        var response = new DisponibiliteResponse
        {
            PrestationId = result.PrestationId,
            IsAvailable = result.IsAvailable,
            AvailableSlots = result.AvailableSlots.Select(slot => new CreneauDisponibleDto
            {
                Date = slot.Date,
                CapaciteMax = slot.CapaciteMax,
                CapaciteReservee = slot.CapaciteReservee,
                CapaciteDisponible = slot.CapaciteDisponible
            }).ToList(),
            Message = result.Message
        };

        return Results.Ok(response);
    }
}