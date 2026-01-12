using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation;

/// <summary>
/// Validates a specific constraint type for Activity-Slot-Resource compatibility
/// </summary>
public interface IConstraintValidator
{
    /// <summary>
    /// The constraint key this validator handles (e.g., "preferred_weekdays", "required_capacity")
    /// </summary>
    string ConstraintKey { get; }

    /// <summary>
    /// Validates the constraint for the given Activity, Slot, and Resource
    /// </summary>
    /// <param name="constraint">The constraint to validate</param>
    /// <param name="activity">The activity being assigned</param>
    /// <param name="slot">The time slot</param>
    /// <param name="resource">The resource</param>
    /// <returns>A ConstraintViolation if the constraint is violated, null if satisfied</returns>
    Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    );
}
