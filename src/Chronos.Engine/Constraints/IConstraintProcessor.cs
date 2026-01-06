namespace Chronos.Engine.Constraints;

/// <summary>
/// Processes activity constraints to determine excluded slots
/// </summary>
public interface IConstraintProcessor
{
    /// <summary>
    /// Get all slot IDs that should be excluded for a given activity
    /// based on its constraints
    /// </summary>
    Task<HashSet<Guid>> GetExcludedSlotIdsAsync(Guid activityId, Guid organizationId);
}
