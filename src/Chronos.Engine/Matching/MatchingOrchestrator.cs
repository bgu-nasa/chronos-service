using Chronos.Domain.Schedule.Messages;

namespace Chronos.Engine.Matching;

/// <summary>
/// Routes scheduling requests to the appropriate matching strategy
/// </summary>
public class MatchingOrchestrator
{
    private readonly IEnumerable<IMatchingStrategy> _strategies;
    private readonly ILogger<MatchingOrchestrator> _logger;

    public MatchingOrchestrator(
        IEnumerable<IMatchingStrategy> strategies,
        ILogger<MatchingOrchestrator> logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public async Task<SchedulingResult> ExecuteAsync(
        object request,
        SchedulingMode mode,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Routing scheduling request of type {RequestType} to {Mode} strategy",
            request.GetType().Name,
            mode);

        var strategy = _strategies.FirstOrDefault(s => s.Mode == mode);

        if (strategy == null)
        {
            var errorMessage = $"No strategy found for mode: {mode}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogDebug(
            "Selected strategy: {StrategyType}",
            strategy.GetType().Name);

        return await strategy.ExecuteAsync(request, cancellationToken);
    }
}
