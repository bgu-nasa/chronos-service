using Chronos.Domain.Schedule.Messages;

namespace Chronos.Engine.Matching;

/// <summary>
/// Strategy interface for different matching algorithms
/// </summary>
public interface IMatchingStrategy
{
    /// <summary>
    /// The scheduling mode this strategy handles
    /// </summary>
    SchedulingMode Mode { get; }

    /// <summary>
    /// Execute the matching algorithm
    /// </summary>
    Task<SchedulingResult> ExecuteAsync(object request, CancellationToken cancellationToken);
}
