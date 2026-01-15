using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a slot's weekday matches the activity's preferred weekdays
/// Constraint Key: "preferred_weekdays"
/// Value Format: "Monday,Wednesday,Friday" (comma-separated weekday names)
/// Type: Soft constraint (warning if not matched)
/// </summary>
public class PreferredWeekdaysValidator(ILogger<PreferredWeekdaysValidator> logger)
    : IConstraintValidator
{
    private readonly ILogger<PreferredWeekdaysValidator> _logger = logger;

    public string ConstraintKey => "preferred_weekdays";

    public Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            // Parse comma-separated weekdays
            var preferredWeekdays = constraint
                .Value.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )
                .Select(w => w.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!preferredWeekdays.Any())
            {
                _logger.LogWarning(
                    "Empty preferred weekdays constraint for Activity {ActivityId}",
                    activity.Id
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Check if slot's weekday is in the preferred list
            if (!preferredWeekdays.Contains(slot.Weekday))
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Soft,
                        Severity = ViolationSeverity.Warning,
                        Message =
                            $"Slot weekday '{slot.Weekday}' is not in preferred weekdays: {string.Join(", ", preferredWeekdays)}",
                        Details =
                            $"Preferred weekdays: {constraint.Value}, Actual weekday: {slot.Weekday}",
                    }
                );
            }

            // No violation - weekday matches preference
            return Task.FromResult<ConstraintViolation?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating preferred_weekdays constraint for Activity {ActivityId}",
                activity.Id
            );

            // Return error violation for malformed constraint
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
}
