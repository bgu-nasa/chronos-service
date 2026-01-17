using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation;

/// <summary>
/// Main implementation of constraint evaluation for Activity-Slot-Resource compatibility
/// </summary>
public class ConstraintEvaluator : IConstraintEvaluator
{
    private readonly IActivityConstraintRepository _constraintRepository;
    private readonly IEnumerable<IConstraintValidator> _validators;
    private readonly ILogger<ConstraintEvaluator> _logger;

    public ConstraintEvaluator(
        IActivityConstraintRepository constraintRepository,
        IEnumerable<IConstraintValidator> validators,
        ILogger<ConstraintEvaluator> logger
    )
    {
        _constraintRepository = constraintRepository;
        _validators = validators;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> CanAssignAsync(Activity activity, Slot slot, Resource resource)
    {
        var violations = await GetViolationsAsync(activity, slot, resource);

        // Assignment is valid only if there are no hard constraint violations
        var hasHardViolations = violations.Any(v => v.ViolationType == ViolationType.Hard);

        return !hasHardViolations;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConstraintViolation>> GetViolationsAsync(
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        var violations = new List<ConstraintViolation>();

        // Load all constraints for this activity
        var constraints = await _constraintRepository.GetByActivityIdAsync(activity.Id);

        _logger.LogDebug(
            "Evaluating {ConstraintCount} constraints for Activity {ActivityId} with Slot {SlotId} and Resource {ResourceId}",
            constraints.Count,
            activity.Id,
            slot.Id,
            resource.Id
        );

        foreach (var constraint in constraints)
        {
            // Find validator for this constraint type
            var validator = _validators.FirstOrDefault(v => v.ConstraintKey == constraint.Key);

            if (validator == null)
            {
                _logger.LogWarning(
                    "No validator found for constraint key '{ConstraintKey}'. Skipping.",
                    constraint.Key
                );
                continue;
            }

            // Validate the constraint
            var violation = await validator.ValidateAsync(constraint, activity, slot, resource);

            if (violation != null)
            {
                violations.Add(violation);

                _logger.LogDebug(
                    "Constraint violation detected: {ConstraintKey}={ConstraintValue}, Severity={Severity}, Message={Message}",
                    violation.ConstraintKey,
                    violation.ConstraintValue,
                    violation.Severity,
                    violation.Message
                );
            }
        }

        _logger.LogInformation(
            "Evaluation complete: {ViolationCount} violations found ({HardCount} hard, {SoftCount} soft)",
            violations.Count,
            violations.Count(v => v.ViolationType == ViolationType.Hard),
            violations.Count(v => v.ViolationType == ViolationType.Soft)
        );

        return violations;
    }
}
