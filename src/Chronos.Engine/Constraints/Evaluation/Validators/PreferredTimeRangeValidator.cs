using System.Text.RegularExpressions;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a slot falls within preferred time ranges
/// Constraint Key: "preferred_timerange"
/// Value Format: "Monday 09:30 - 11:00" or multiple entries separated by commas/newlines
/// Type: Soft constraint (warning if not matched)
/// </summary>
public class PreferredTimeRangeValidator(ILogger<PreferredTimeRangeValidator> logger) : IConstraintValidator
{
    private readonly ILogger<PreferredTimeRangeValidator> _logger = logger;

    public string ConstraintKey => "preferred_timerange";

    public Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(constraint.Value))
            {
                _logger.LogWarning(
                    "Empty preferred_timerange constraint for Activity {ActivityId}",
                    activity.Id
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Parse multiple preferred time ranges (comma or newline separated)
            var preferredRanges = ParsePreferredRanges(constraint.Value);

            if (!preferredRanges.Any())
            {
                _logger.LogWarning(
                    "No valid preferred time ranges found for Activity {ActivityId}: {Value}",
                    activity.Id,
                    constraint.Value
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Check if slot falls within any preferred time range
            foreach (var preferredRange in preferredRanges)
            {
                // Check if weekday matches (case-insensitive)
                if (!string.Equals(slot.Weekday, preferredRange.Weekday, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Different weekday, check next range
                }

                // Check if slot falls entirely within the preferred time range
                // Slot is within preferred range if: slotStart >= preferredStart AND slotEnd <= preferredEnd
                if (slot.FromTime >= preferredRange.StartTime && slot.ToTime <= preferredRange.EndTime)
                {
                    // Slot matches a preferred range - no violation
                    return Task.FromResult<ConstraintViolation?>(null);
                }
            }

            // No preferred range matches - return warning
            var preferredRangesForWeekday = preferredRanges
                .Where(r => string.Equals(r.Weekday, slot.Weekday, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (preferredRangesForWeekday.Any())
            {
                var rangesList = string.Join(", ", preferredRangesForWeekday.Select(r => $"{r.StartTime:hh\\:mm}-{r.EndTime:hh\\:mm}"));
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Soft,
                        Severity = ViolationSeverity.Warning,
                        Message =
                            $"Slot time range ({slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}) on {slot.Weekday} does not fall within preferred time ranges ({rangesList})",
                        Details =
                            $"Preferred ranges for {slot.Weekday}: {rangesList}, Slot: {slot.Weekday} {slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}",
                    }
                );
            }
            else
            {
                // No preferred ranges for this weekday
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Soft,
                        Severity = ViolationSeverity.Warning,
                        Message =
                            $"Slot weekday '{slot.Weekday}' does not have any preferred time ranges",
                        Details =
                            $"Slot: {slot.Weekday} {slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}, Preferred ranges: {constraint.Value}",
                    }
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating preferred_timerange constraint for Activity {ActivityId}",
                activity.Id
            );

            return Task.FromResult<ConstraintViolation?>(
                new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message = "Invalid constraint format",
                    Details = ex.Message,
                }
            );
        }
    }

    private List<PreferredTimeRange> ParsePreferredRanges(string value)
    {
        var ranges = new List<PreferredTimeRange>();

        // Split by comma or newline
        var entries = value.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Pattern: "Weekday HH:mm - HH:mm" or "Weekday HH:mm-HH:mm"
        // Examples: "Monday 09:30 - 11:00", "Tuesday 13:00-15:00"
        var pattern = @"^(\w+)\s+(\d{1,2}:\d{2})\s*-\s*(\d{1,2}:\d{2})$";

        foreach (var entry in entries)
        {
            var trimmedEntry = entry.Trim();
            if (string.IsNullOrWhiteSpace(trimmedEntry))
            {
                continue;
            }

            var match = Regex.Match(trimmedEntry, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                _logger.LogWarning(
                    "Invalid preferred_timerange format: '{Entry}'. Expected format: 'Weekday HH:mm - HH:mm'",
                    trimmedEntry
                );
                continue;
            }

            var weekday = match.Groups[1].Value;
            var startTimeStr = match.Groups[2].Value;
            var endTimeStr = match.Groups[3].Value;

            if (!TimeSpan.TryParse(startTimeStr, out var startTime) ||
                !TimeSpan.TryParse(endTimeStr, out var endTime))
            {
                _logger.LogWarning(
                    "Invalid time format in preferred_timerange: '{Entry}'. Use HH:mm format",
                    trimmedEntry
                );
                continue;
            }

            if (startTime >= endTime)
            {
                _logger.LogWarning(
                    "Start time must be before end time in preferred_timerange: '{Entry}'",
                    trimmedEntry
                );
                continue;
            }

            ranges.Add(new PreferredTimeRange
            {
                Weekday = weekday,
                StartTime = startTime,
                EndTime = endTime
            });
        }

        return ranges;
    }

    private class PreferredTimeRange
    {
        public string Weekday { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
