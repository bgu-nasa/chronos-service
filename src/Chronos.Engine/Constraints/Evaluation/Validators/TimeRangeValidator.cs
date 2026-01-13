using System.Text.Json;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a slot's time range falls within the specified time range
/// Constraint Key: "time_range"
/// Value Format: {"start": "08:00", "end": "17:00"} (JSON with start/end times in HH:mm format)
/// Type: Hard constraint (error if violated)
/// </summary>
public class TimeRangeValidator : IConstraintValidator
{
    private readonly ILogger<TimeRangeValidator> _logger;

    public string ConstraintKey => "time_range";

    public TimeRangeValidator(ILogger<TimeRangeValidator> logger)
    {
        _logger = logger;
    }

    public Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            // Parse JSON constraint value
            var timeRange = JsonSerializer.Deserialize<TimeRangeConstraint>(constraint.Value);

            if (
                timeRange == null
                || string.IsNullOrEmpty(timeRange.Start)
                || string.IsNullOrEmpty(timeRange.End)
            )
            {
                _logger.LogWarning(
                    "Invalid time_range constraint format for Activity {ActivityId}: {Value}",
                    activity.Id,
                    constraint.Value
                );

                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message =
                            "Invalid time range format. Expected JSON: {\"start\": \"HH:mm\", \"end\": \"HH:mm\"}",
                        Details = constraint.Value,
                    }
                );
            }

            // Parse time strings to TimeSpan
            if (
                !TimeSpan.TryParse(timeRange.Start, out var startTime)
                || !TimeSpan.TryParse(timeRange.End, out var endTime)
            )
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message = "Invalid time format. Use HH:mm format (e.g., \"08:00\")",
                        Details = $"Start: {timeRange.Start}, End: {timeRange.End}",
                    }
                );
            }

            // Check if slot's time range is within the constraint
            // Slot must start at or after constraint start AND end at or before constraint end
            if (slot.FromTime < startTime || slot.ToTime > endTime)
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message =
                            $"Slot time range ({slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}) is outside allowed range ({startTime:hh\\:mm}-{endTime:hh\\:mm})",
                        Details =
                            $"Allowed: {startTime:hh\\:mm}-{endTime:hh\\:mm}, Slot: {slot.FromTime:hh\\:mm}-{slot.ToTime:hh\\:mm}",
                    }
                );
            }

            // No violation - slot is within time range
            return Task.FromResult<ConstraintViolation?>(null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "JSON parsing error for time_range constraint, Activity {ActivityId}",
                activity.Id
            );

            return Task.FromResult<ConstraintViolation?>(
                new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message = "Invalid JSON format",
                    Details = ex.Message,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating time_range constraint for Activity {ActivityId}",
                activity.Id
            );

            return Task.FromResult<ConstraintViolation?>(
                new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message = "Constraint validation error",
                    Details = ex.Message,
                }
            );
        }
    }

    private class TimeRangeConstraint
    {
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
    }
}
