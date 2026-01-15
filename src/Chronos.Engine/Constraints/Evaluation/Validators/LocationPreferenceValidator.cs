using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a resource's location matches the activity's preferred locations
/// Constraint Key: "location_preference"
/// Value Format: "Building A,Building B,Building C" (comma-separated location names)
/// Type: Soft constraint (warning if not matched)
/// </summary>
public class LocationPreferenceValidator(ILogger<LocationPreferenceValidator> logger)
    : IConstraintValidator
{
    private readonly ILogger<LocationPreferenceValidator> _logger = logger;

    public string ConstraintKey => "location_preference";

    public Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            // Parse comma-separated locations
            var preferredLocations = constraint
                .Value.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )
                .Select(l => l.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!preferredLocations.Any())
            {
                _logger.LogWarning(
                    "Empty location preference constraint for Activity {ActivityId}",
                    activity.Id
                );
                return Task.FromResult<ConstraintViolation?>(null);
            }

            // Check if resource's location is in the preferred list
            if (!preferredLocations.Contains(resource.Location))
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Soft,
                        Severity = ViolationSeverity.Warning,
                        Message =
                            $"Resource location '{resource.Location}' is not in preferred locations: {string.Join(", ", preferredLocations)}",
                        Details =
                            $"Preferred locations: {constraint.Value}, Resource location: {resource.Location}",
                    }
                );
            }

            // No violation - location matches preference
            return Task.FromResult<ConstraintViolation?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating location_preference constraint for Activity {ActivityId}",
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
}
