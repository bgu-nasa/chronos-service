using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Handlers;

/// <summary>
/// Example handler: Excludes all slots on a specific weekday
/// Constraint: Key = "excluded_weekday", Value = "Monday" (or any weekday)
/// </summary>
public class ExampleExcludedWeekdayConstraintHandler : IConstraintHandler
{
    public string ConstraintKey => "excluded_weekday";

    private readonly ISlotRepository _slotRepository;
    private readonly ILogger<ExampleExcludedWeekdayConstraintHandler> _logger;

    public ExampleExcludedWeekdayConstraintHandler(
        ISlotRepository slotRepository,
        ILogger<ExampleExcludedWeekdayConstraintHandler> logger)
    {
        _slotRepository = slotRepository;
        _logger = logger;
    }

    public async Task<HashSet<Guid>> GetExcludedSlotIdsAsync(
        ActivityConstraint constraint,
        Guid organizationId)
    {
        var excludedWeekday = constraint.Value;

        _logger.LogDebug(
            "Excluding all slots for weekday: {Weekday}",
            excludedWeekday);

        // Load all slots (in production, filter by organization/period)
        var allSlots = await _slotRepository.GetAllAsync();

        // Filter slots matching the excluded weekday
        var excludedSlots = allSlots
            .Where(s => s.Weekday.Equals(excludedWeekday, StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Id)
            .ToHashSet();

        _logger.LogInformation(
            "Excluded {SlotCount} slots for weekday {Weekday}",
            excludedSlots.Count,
            excludedWeekday);

        return excludedSlots;
    }
}
