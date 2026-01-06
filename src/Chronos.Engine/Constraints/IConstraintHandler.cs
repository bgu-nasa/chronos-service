using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints;

/// <summary>
/// Handles a specific type of activity constraint
/// </summary>
public interface IConstraintHandler
{
    /// <summary>
    /// The constraint key this handler processes (e.g., "excluded_weekday")
    /// </summary>
    string ConstraintKey { get; }

    /// <summary>
    /// Process the constraint and return slot IDs to exclude
    /// </summary>
    Task<HashSet<Guid>> GetExcludedSlotIdsAsync(
        ActivityConstraint constraint,
        Guid organizationId);
}
