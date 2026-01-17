namespace Chronos.Domain.Constraints;

/// <summary>
/// Defines the severity level of a constraint violation
/// </summary>
public enum ViolationSeverity
{
    /// <summary>
    /// Informational message - no impact on assignment validity
    /// </summary>
    Info,

    /// <summary>
    /// Warning - soft constraint not satisfied, but assignment is still valid
    /// </summary>
    Warning,

    /// <summary>
    /// Error - hard constraint violated, assignment is invalid
    /// </summary>
    Error,
}
