using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Handlers;

/// <summary>
/// Example constraint handler that excludes all slots on specified weekdays
/// Constraint Value format: "Monday,Wednesday,Friday" (comma-separated weekdays)
/// </summary>
public class ExampleExcludedWeekdayConstraintHandler : IConstraintHandler
{
    public string ConstraintKey => "excluded_weekdays";

    private readonly ISlotRepository _slotRepository;
    private readonly ILogger<ExampleExcludedWeekdayConstraintHandler> _logger;

    public ExampleExcludedWeekdayConstraintHandler(
        ISlotRepository slotRepository,
        ILogger<ExampleExcludedWeekdayConstraintHandler> logger)
    {
        _slotRepository = slotRepository;
        _logger = logger;
    }

    public async Task<HashSet<Guid>> ProcessConstraintAsync(
        ActivityConstraint constraint,
        Guid organizationId)
    {
        _logger.LogDebug(
            "Processing excluded_weekdays constraint. Value: {Value}",
            constraint.Value);

        // Parse comma-separated weekdays
        var excludedWeekdays = constraint.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(w => w.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!excludedWeekdays.Any())
        {
            _logger.LogWarning(
                "No weekdays found in constraint value: {Value}",
                constraint.Value);
            return new HashSet<Guid>();
        }

        // Get all slots for the organization
        var allSlots = await _slotRepository.GetAllAsync();

        // Filter slots matching excluded weekdays
        var excludedSlots = allSlots
            .Where(s => excludedWeekdays.Contains(s.Weekday))
            .Select(s => s.Id)
            .ToHashSet();

        _logger.LogInformation(
            "Excluded {SlotCount} slots for weekdays: {Weekdays}",
            excludedSlots.Count,
            string.Join(", ", excludedWeekdays));

        return excludedSlots;
    }
}
