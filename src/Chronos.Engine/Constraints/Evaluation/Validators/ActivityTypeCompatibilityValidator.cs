using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Constraints;
using Chronos.Domain.Resources;
using Chronos.Domain.Schedule;

namespace Chronos.Engine.Constraints.Evaluation.Validators;

/// <summary>
/// Validates that a resource's type is compatible with the activity type
/// Constraint Key: "compatible_resource_types"
/// Value Format: "Lecture Hall,Seminar Room,Laboratory" (comma-separated resource type names)
/// Type: Hard constraint (error if violated)
/// </summary>
public class ActivityTypeCompatibilityValidator : IConstraintValidator
{
    private readonly IResourceTypeRepository _resourceTypeRepository;
    private readonly ILogger<ActivityTypeCompatibilityValidator> _logger;

    public string ConstraintKey => "compatible_resource_types";

    public ActivityTypeCompatibilityValidator(
        IResourceTypeRepository resourceTypeRepository,
        ILogger<ActivityTypeCompatibilityValidator> logger
    )
    {
        _resourceTypeRepository = resourceTypeRepository;
        _logger = logger;
    }

    public async Task<ConstraintViolation?> ValidateAsync(
        ActivityConstraint constraint,
        Activity activity,
        Slot slot,
        Resource resource
    )
    {
        try
        {
            // Parse comma-separated resource type names
            var compatibleTypeNames = constraint
                .Value.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )
                .Select(t => t.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!compatibleTypeNames.Any())
            {
                _logger.LogWarning(
                    "Empty compatible resource types constraint for Activity {ActivityId}",
                    activity.Id
                );

                return new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message = "No compatible resource types specified",
                    Details = constraint.Value,
                };
            }

            // Load the resource's type information
            var resourceType = await _resourceTypeRepository.GetByIdAsync(resource.ResourceTypeId);

            if (resourceType == null)
            {
                _logger.LogWarning(
                    "Resource type {ResourceTypeId} not found for Resource {ResourceId}",
                    resource.ResourceTypeId,
                    resource.Id
                );

                return new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message =
                        $"Resource type information not found for resource '{resource.Identifier}'",
                    Details = $"ResourceTypeId: {resource.ResourceTypeId}",
                };
            }

            // Check if resource's type is in the compatible list
            if (!compatibleTypeNames.Contains(resourceType.Type))
            {
                return new ConstraintViolation
                {
                    ConstraintKey = ConstraintKey,
                    ConstraintValue = constraint.Value,
                    ViolationType = ViolationType.Hard,
                    Severity = ViolationSeverity.Error,
                    Message =
                        $"Resource type '{resourceType.Type}' is not compatible with activity type '{activity.ActivityType}'",
                    Details =
                        $"Compatible types: {constraint.Value}, Resource type: {resourceType.Type}",
                };
            }

            // No violation - resource type is compatible
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating compatible_resource_types constraint for Activity {ActivityId}",
                activity.Id
            );

            return new ConstraintViolation
            {
                ConstraintKey = ConstraintKey,
                ConstraintValue = constraint.Value,
                ViolationType = ViolationType.Hard,
                Severity = ViolationSeverity.Error,
                Message = "Constraint validation error",
                Details = ex.Message,
            };
        }
    }
}
