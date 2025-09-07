using MediatR;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.TaskWorkerProcess.ProcessExpiredReservations;
using Quartz;

namespace PetBoarding_TaskWorker.Jobs;

/// <summary>
/// Job Quartz pour traiter les réservations expirées et libérer leurs créneaux
/// </summary>
[DisallowConcurrentExecution]
public sealed class ProcessExpiredReservationsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessExpiredReservationsJob> _logger;

    public ProcessExpiredReservationsJob(
        IMediator mediator,
        ILogger<ProcessExpiredReservationsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var startTime = DateTimeOffset.Now;
        _logger.LogInformation("Starting ProcessExpiredReservationsJob at {StartTime}", startTime);

        try
        {
            var command = new ProcessExpiredReservationsCommand();
            var result = await _mediator.Send(command, context.CancellationToken);

            if (result.IsSuccess)
            {
                var duration = DateTimeOffset.Now - startTime;
                _logger.LogInformation("ProcessExpiredReservationsJob completed successfully in {Duration}ms. Processed {Count} expired reservations",
                    duration.TotalMilliseconds, result.Value);
            }
            else
            {
                _logger.LogError("ProcessExpiredReservationsJob failed: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Message)));
                throw new JobExecutionException($"Failed to process expired reservations: {string.Join(", ", result.Errors.Select(e => e.Message))}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProcessExpiredReservationsJob encountered an unexpected error");
            throw new JobExecutionException("ProcessExpiredReservationsJob failed with unexpected error", ex);
        }
    }
}