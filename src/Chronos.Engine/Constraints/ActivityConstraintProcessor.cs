using Chronos.Data.Repositories.Schedule;

namespace Chronos.Engine.Constraints;

/// <summary>
/// Orchestrates constraint processing using registered handlers
/// </summary>
public class ActivityConstraintProcessor : IConstraintProcessor
{
    private readonly IActivityConstraintRepository _constraintRepository;
    private readonly IEnumerable<IConstraintHandler> _handlers;
    private readonly ILogger<ActivityConstraintProcessor> _logger;

    public ActivityConstraintProcessor(
        IActivityConstraintRepository constraintRepository,
        IEnumerable<IConstraintHandler> handlers,
        ILogger<ActivityConstraintProcessor> logger)
    {
        _constraintRepository = constraintRepository;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task<HashSet<Guid>> GetExcludedSlotIdsAsync(Guid activityId, Guid organizationId)
    {
        var excludedSlots = new HashSet<Guid>();

        // Load all constraints for this activity
        var constraints = await _constraintRepository.GetByActivityIdAsync(activityId);

        _logger.LogDebug(
            "Processing {ConstraintCount} constraints for Activity {ActivityId}",
            constraints.Count,
            activityId);

        foreach (var constraint in constraints)
        {
            // Find handler for this constraint type
            var handler = _handlers.FirstOrDefault(h => h.ConstraintKey == constraint.Key);

            if (handler == null)
            {
                _logger.LogWarning(
                    "No handler found for constraint key '{ConstraintKey}'. Skipping.",
                    constraint.Key);
                continue;
            }

            // Process constraint
            var excludedByThisConstraint = await handler.ProcessConstraintAsync(
                constraint,
                organizationId);

            _logger.LogDebug(
                "Constraint {ConstraintKey}={ConstraintValue} excluded {SlotCount} slots",
                constraint.Key,
                constraint.Value,
                excludedByThisConstraint.Count);

            // Add to overall excluded set
            excludedSlots.UnionWith(excludedByThisConstraint);
        }

        _logger.LogInformation(
            "Total {ExcludedCount} slots excluded for Activity {ActivityId}",
            excludedSlots.Count,
            activityId);

        return excludedSlots;
    }
}
