using MediatR;
using PetBoarding_Application.Core.Prestations.GetPrestationSchedule.Models;

namespace PetBoarding_Application.Core.Prestations.GetPrestationSchedule;

public record GetPrestationScheduleQuery(
    string PrestationId,
    int Year,
    int? Month = null
) : IRequest<PrestationScheduleResult>;