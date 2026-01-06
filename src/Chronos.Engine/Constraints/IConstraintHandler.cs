using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints;

/// <summary>
/// Handler interface for processing specific constraint types
/// Allows extensibility for adding new constraint types
/// </summary>
public interface IConstraintHandler
{
    /// <summary>
    /// The constraint key this handler processes (e.g., "no_monday_morning", "required_capacity")
    /// </summary>
    string ConstraintKey { get; }

    /// <summary>
    /// Process the constraint and return the set of Slot IDs that should be excluded
    /// </summary>
    Task<HashSet<Guid>> ProcessConstraintAsync(
        ActivityConstraint constraint,
        Guid organizationId);
}
