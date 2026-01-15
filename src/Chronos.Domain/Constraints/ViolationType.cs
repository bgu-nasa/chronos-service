namespace Chronos.Domain.Constraints;

/// <summary>
/// Defines the type of constraint violation
/// </summary>
public enum ViolationType
{
    /// <summary>
    /// Hard constraint - must be satisfied for assignment to be valid
    /// </summary>
    Hard,

    /// <summary>
    /// Soft constraint - preference that should be satisfied when possible
    /// </summary>
    Soft,
}
