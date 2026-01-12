namespace Chronos.Domain.Constraints;

/// <summary>
/// Represents a violation of an activity constraint
/// </summary>
public class ConstraintViolation
{
    /// <summary>
    /// The constraint key that was violated
    /// </summary>
    public required string ConstraintKey { get; set; }

    /// <summary>
    /// The constraint value that was checked
    /// </summary>
    public required string ConstraintValue { get; set; }

    /// <summary>
    /// Type of violation (Hard or Soft)
    /// </summary>
    public ViolationType ViolationType { get; set; }

    /// <summary>
    /// Severity of the violation
    /// </summary>
    public ViolationSeverity Severity { get; set; }

    /// <summary>
    /// Human-readable description of the violation
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Additional context or details about the violation
    /// </summary>
    public string? Details { get; set; }
}
