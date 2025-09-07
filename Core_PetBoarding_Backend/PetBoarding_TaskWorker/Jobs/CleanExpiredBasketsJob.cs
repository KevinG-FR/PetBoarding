using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.TaskWorkerProcess.ProcessExpiredBaskets;
using Quartz;

namespace PetBoarding_TaskWorker.Jobs;

/// <summary>
/// Job Quartz pour nettoyer les paniers expirés et libérer leurs créneaux
/// </summary>
[DisallowConcurrentExecution]
public sealed class CleanExpiredBasketsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CleanExpiredBasketsJob> _logger;

    public CleanExpiredBasketsJob(
        IMediator mediator,
        IConfiguration configuration,
        ILogger<CleanExpiredBasketsJob> logger)
    {
        _mediator = mediator;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var startTime = DateTimeOffset.Now;
        _logger.LogInformation("Starting CleanExpiredBasketsJob at {StartTime}", startTime);

        try
        {
            // Récupérer la configuration d'expiration des paniers
            var expirationMinutes = _configuration.GetValue<int>("TaskWorker:BasketExpirationMinutes", 30);
            
            var command = new ProcessExpiredBasketsCommand(expirationMinutes);
            var result = await _mediator.Send(command, context.CancellationToken);

            if (result.IsSuccess)
            {
                var duration = DateTimeOffset.Now - startTime;
                _logger.LogInformation("CleanExpiredBasketsJob completed successfully in {Duration}ms. Processed {Count} expired baskets",
                    duration.TotalMilliseconds, result.Value);
            }
            else
            {
                _logger.LogError("CleanExpiredBasketsJob failed: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Message)));
                throw new JobExecutionException($"Failed to process expired baskets: {string.Join(", ", result.Errors.Select(e => e.Message))}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CleanExpiredBasketsJob encountered an unexpected error");
            throw new JobExecutionException("CleanExpiredBasketsJob failed with unexpected error", ex);
        }
    }
}