using Chronos.Domain.Schedule.Messages;

namespace Chronos.Engine.Matching;

/// <summary>
/// Routes scheduling requests to the appropriate matching strategy (Batch or Online)
/// </summary>
public class MatchingOrchestrator(
    IEnumerable<IMatchingStrategy> strategies,
    ILogger<MatchingOrchestrator> logger
)
{
    private readonly IEnumerable<IMatchingStrategy> _strategies = strategies;
    private readonly ILogger<MatchingOrchestrator> _logger = logger;

    public async Task<SchedulingResult> ExecuteAsync(
        object request,
        SchedulingMode mode,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Orchestrating scheduling request with mode {Mode}, request type {RequestType}",
            mode,
            request.GetType().Name
        );

        var strategy = _strategies.FirstOrDefault(s => s.Mode == mode);

        if (strategy == null)
        {
            var error = $"No strategy found for mode {mode}";
            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        _logger.LogInformation(
            "Using strategy {StrategyType} for mode {Mode}",
            strategy.GetType().Name,
            mode
        );

        return await strategy.ExecuteAsync(request, cancellationToken);
    }
}
