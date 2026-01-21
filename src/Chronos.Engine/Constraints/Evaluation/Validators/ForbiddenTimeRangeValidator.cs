using System.Text.RegularExpressions;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a slot does not overlap with forbidden time ranges
/// Constraint Key: "forbidden_timerange"
/// Value Format: "Monday 09:30 - 11:00" or multiple entries separated by commas/newlines
/// Type: Hard constraint (error if violated)
/// </summary>
public class ForbiddenTimeRangeValidator(ILogger<ForbiddenTimeRangeValidator> logger) : IConstraintValidator
{
    private readonly ILogger<ForbiddenTimeRangeValidator> _logger = logger;

    public string ConstraintKey => "forbidden_timerange";

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
                    "Empty forbidden_timerange constraint for Activity {ActivityId}",
                    activity.Id
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Parse multiple forbidden time ranges (comma or newline separated)
            var forbiddenRanges = ParseForbiddenRanges(constraint.Value);

            if (!forbiddenRanges.Any())
            {
                _logger.LogWarning(
                    "No valid forbidden time ranges found for Activity {ActivityId}: {Value}",
                    activity.Id,
                    constraint.Value
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Check if slot overlaps with any forbidden time range
            foreach (var forbiddenRange in forbiddenRanges)
            {
                // Check if weekday matches (case-insensitive)
                if (!string.Equals(slot.Weekday, forbiddenRange.Weekday, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Different weekday, no conflict
                }

                // Check if time ranges overlap
                // Two time ranges overlap if: slotStart < forbiddenEnd AND slotEnd > forbiddenStart
                if (slot.FromTime < forbiddenRange.EndTime && slot.ToTime > forbiddenRange.StartTime)
                {
                    return Task.FromResult<ConstraintViolation?>(
                        new ConstraintViolation
                        {
                            ConstraintKey = ConstraintKey,
                            ConstraintValue = constraint.Value,
                            ViolationType = ViolationType.Hard,
                            Severity = ViolationSeverity.Error,
                            Message =
                                $"Slot time range ({slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}) on {slot.Weekday} overlaps with forbidden time range ({forbiddenRange.StartTime:hh\\:mm}-{forbiddenRange.EndTime:hh\\:mm})",
                            Details =
                                $"Forbidden: {forbiddenRange.Weekday} {forbiddenRange.StartTime:hh\\:mm}-{forbiddenRange.EndTime:hh\\:mm}, Slot: {slot.Weekday} {slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}",
                        }
                    );
                }
            }

            // No violation - slot does not overlap with any forbidden time range
            return Task.FromResult<ConstraintViolation?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating forbidden_timerange constraint for Activity {ActivityId}",
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

    private List<ForbiddenTimeRange> ParseForbiddenRanges(string value)
    {
        var ranges = new List<ForbiddenTimeRange>();

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
                    "Invalid forbidden_timerange format: '{Entry}'. Expected format: 'Weekday HH:mm - HH:mm'",
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
                    "Invalid time format in forbidden_timerange: '{Entry}'. Use HH:mm format",
                    trimmedEntry
                );
                continue;
            }

            if (startTime >= endTime)
            {
                _logger.LogWarning(
                    "Start time must be before end time in forbidden_timerange: '{Entry}'",
                    trimmedEntry
                );
                continue;
            }

            ranges.Add(new ForbiddenTimeRange
            {
                Weekday = weekday,
                StartTime = startTime,
                EndTime = endTime
            });
        }

        return ranges;
    }

    private class ForbiddenTimeRange
    {
        public string Weekday { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
