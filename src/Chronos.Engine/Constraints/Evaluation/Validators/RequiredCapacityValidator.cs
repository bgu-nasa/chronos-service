using System.Text.Json;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a resource's capacity meets the activity's requirements
/// Constraint Key: "required_capacity"
/// Value Format: {"min": 30} or {"min": 20, "max": 50} (JSON with min/max capacity)
/// Type: Hard constraint (error if violated)
/// </summary>
public class RequiredCapacityValidator(ILogger<RequiredCapacityValidator> logger)
    : IConstraintValidator
{
    private readonly ILogger<RequiredCapacityValidator> _logger = logger;

    public string ConstraintKey => "required_capacity";

    public Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            // Parse JSON constraint value (case-insensitive to allow both "min"/"Min" and "max"/"Max")
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var capacityConstraint = JsonSerializer.Deserialize<CapacityConstraint>(
                constraint.Value,
                options
            );

            if (capacityConstraint == null)
            {
                _logger.LogWarning(
                    "Invalid required_capacity constraint format for Activity {ActivityId}: {Value}",
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
                            "Invalid capacity format. Expected JSON: {\"min\": 30} or {\"min\": 20, \"max\": 50}",
                        Details = constraint.Value,
                    }
                );
            }

            // Check if resource has capacity information
            if (!resource.Capacity.HasValue)
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message =
                            $"Resource '{resource.Identifier}' does not have capacity information",
                        Details = $"Required capacity: {constraint.Value}, Resource capacity: null",
                    }
                );
            }

            var resourceCapacity = resource.Capacity.Value;

            // Check minimum capacity
            if (capacityConstraint.Min.HasValue && resourceCapacity < capacityConstraint.Min.Value)
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message =
                            $"Resource capacity ({resourceCapacity}) is below minimum required ({capacityConstraint.Min.Value})",
                        Details =
                            $"Required min: {capacityConstraint.Min.Value}, Resource capacity: {resourceCapacity}",
                    }
                );
            }

            // Check maximum capacity
            if (capacityConstraint.Max.HasValue && resourceCapacity > capacityConstraint.Max.Value)
            {
                return Task.FromResult<ConstraintViolation?>(
                    new ConstraintViolation
                    {
                        ConstraintKey = ConstraintKey,
                        ConstraintValue = constraint.Value,
                        ViolationType = ViolationType.Hard,
                        Severity = ViolationSeverity.Error,
                        Message =
                            $"Resource capacity ({resourceCapacity}) exceeds maximum allowed ({capacityConstraint.Max.Value})",
                        Details =
                            $"Required max: {capacityConstraint.Max.Value}, Resource capacity: {resourceCapacity}",
                    }
                );
            }

            // Optional: Check against expected students if specified
            if (activity.ExpectedStudents.HasValue)
            {
                if (resourceCapacity < activity.ExpectedStudents.Value)
                {
                    return Task.FromResult<ConstraintViolation?>(
                        new ConstraintViolation
                        {
                            ConstraintKey = ConstraintKey,
                            ConstraintValue = constraint.Value,
                            ViolationType = ViolationType.Hard,
                            Severity = ViolationSeverity.Error,
                            Message =
                                $"Resource capacity ({resourceCapacity}) is insufficient for expected students ({activity.ExpectedStudents.Value})",
                            Details =
                                $"Expected students: {activity.ExpectedStudents.Value}, Resource capacity: {resourceCapacity}",
                        }
                    );
                }
            }

            // No violation - capacity is adequate
            return Task.FromResult<ConstraintViolation?>(null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "JSON parsing error for required_capacity constraint, Activity {ActivityId}",
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
                "Error validating required_capacity constraint for Activity {ActivityId}",
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

    private class CapacityConstraint
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}
