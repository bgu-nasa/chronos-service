using Chronos.Domain.Schedule.Messages;

namespace Chronos.Engine.Matching;

public interface IMatchingStrategy
{
    SchedulingMode Mode { get; }
    
    Task<SchedulingResult> ExecuteAsync(object request, CancellationToken cancellationToken);
}
