using Chronos.Data.Repositories.Schedule;

namespace Chronos.Engine.Constraints;

public class ActivityConstraintProcessor : IConstraintProcessor
{
    private readonly IActivityConstraintRepository _activityConstraintRepository;
    private readonly ILogger<ActivityConstraintProcessor> _logger;
    private readonly Dictionary<string, IConstraintHandler> _handlers;

    public ActivityConstraintProcessor(
        IActivityConstraintRepository activityConstraintRepository,
        IEnumerable<IConstraintHandler> handlers,
        ILogger<ActivityConstraintProcessor> logger)
    {
        _activityConstraintRepository = activityConstraintRepository;
        _logger = logger;

        // Build handler registry indexed by constraint key
        _handlers = handlers.ToDictionary(h => h.ConstraintKey, h => h);

        _logger.LogInformation(
            "ActivityConstraintProcessor initialized with {HandlerCount} handlers: {HandlerKeys}",
            _handlers.Count,
            string.Join(", ", _handlers.Keys));
    }

    public async Task<HashSet<Guid>> GetExcludedSlotIdsAsync(Guid activityId, Guid organizationId)
    {
        _logger.LogDebug(
            "Processing constraints for Activity {ActivityId} in Organization {OrganizationId}",
            activityId,
            organizationId);

        var constraints = await _activityConstraintRepository.GetByActivityIdAsync(activityId);

        if (!constraints.Any())
        {
            _logger.LogDebug("No constraints found for Activity {ActivityId}", activityId);
            return new HashSet<Guid>();
        }

        _logger.LogInformation(
            "Found {ConstraintCount} constraints for Activity {ActivityId}",
            constraints.Count,
            activityId);

        var excludedSlots = new HashSet<Guid>();

        foreach (var constraint in constraints)
        {
            if (!_handlers.TryGetValue(constraint.Key, out var handler))
            {
                _logger.LogWarning(
                    "No handler found for constraint key '{ConstraintKey}'. Constraint will be ignored. " +
                    "ActivityConstraintId: {ConstraintId}",
                    constraint.Key,
                    constraint.Id);
                continue;
            }

            try
            {
                var slotsToExclude = await handler.ProcessConstraintAsync(constraint, organizationId);

                _logger.LogDebug(
                    "Constraint {ConstraintKey} (ID: {ConstraintId}) excluded {SlotCount} slots",
                    constraint.Key,
                    constraint.Id,
                    slotsToExclude.Count);

                excludedSlots.UnionWith(slotsToExclude);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing constraint {ConstraintKey} (ID: {ConstraintId}). Constraint will be ignored.",
                    constraint.Key,
                    constraint.Id);
            }
        }

        _logger.LogInformation(
            "Total excluded slots for Activity {ActivityId}: {ExcludedCount}",
            activityId,
            excludedSlots.Count);

        return excludedSlots;
    }
}
