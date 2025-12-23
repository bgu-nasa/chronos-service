using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public class ActivityConstraintService(
    IActivityConstraintRepository activityConstraintRepository,
     ScheduleValidationService validationService,
    ILogger<ActivityConstraintService> logger
) : IActivityConstraintService
{
    public async Task<Guid> CreateActivityConstraintAsync(Guid organizationId, Guid activityId, string key, string value)
    {
        await validationService.ValidateOrganizationAsync(organizationId);
        var constraint = new ActivityConstraint
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            ActivityId = activityId,
            Key = key,
            Value = value
        };
        await activityConstraintRepository.AddAsync(constraint);
        logger.LogInformation("Created ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", constraint.Id, organizationId);
        return constraint.Id;
    }
    
    public async Task<ActivityConstraint> GetActivityConstraintByIdAsync(Guid organizationId, Guid activityConstraintId)
    {
        logger.LogInformation("Retrieving ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
        var ac = await validationService.ValidateAndGetActivityConstraintAsync(organizationId , activityConstraintId);
        logger.LogInformation("Retrieved ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
        return ac;
    }
    
    public async Task<List<ActivityConstraint>> GetAllActivityConstraintsAsync(Guid organizationId)
    {
        logger.LogInformation("Retrieving all ActivityConstraints for Organization {OrganizationId}", organizationId);
        await validationService.ValidateOrganizationAsync(organizationId);
        var constraints = await activityConstraintRepository.GetAllAsync();
        var orgConstraints = constraints.Where(ac => ac.OrganizationId == organizationId).ToList();
        logger.LogInformation("Retrieved {Count} ActivityConstraints for Organization {OrganizationId}", orgConstraints.Count, organizationId);
        return orgConstraints;
    }
    
    public async Task<List<ActivityConstraint>> GetByActivityIdAsync(Guid organizationId, Guid activityId)
    {
        logger.LogInformation("Retrieving ActivityConstraints for Activity {ActivityId} in Organization {OrganizationId}", activityId, organizationId);
        await validationService.ValidateOrganizationAsync(organizationId);
        var constraints = await activityConstraintRepository.GetByActivityIdAsync(activityId);
        var orgConstraints = constraints.Where(ac => ac.OrganizationId == organizationId).ToList();
        logger.LogInformation("Retrieved {Count} ActivityConstraints for Activity {ActivityId} in Organization {OrganizationId}", orgConstraints.Count, activityId, organizationId);
        return orgConstraints;
    }

    public async Task<ActivityConstraint> UpdateActivityConstraintAsync(Guid organizationId, Guid activityConstraintId,
        string key, string value)
    {
        logger.LogInformation("Updating ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
        var constraint = await validationService.ValidateAndGetActivityConstraintAsync(organizationId, activityConstraintId);
        constraint.Key = key;
        constraint.Value = value;
        await activityConstraintRepository.UpdateAsync(constraint);
        logger.LogInformation("Updated ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
        return constraint;
    }
    
    public async Task DeleteActivityConstraintAsync(Guid organizationId, Guid activityConstraintId)
    {
        logger.LogInformation("Deleting ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
        var constraint = await validationService.ValidateAndGetActivityConstraintAsync(organizationId, activityConstraintId);
        await activityConstraintRepository.DeleteAsync(constraint);
        logger.LogInformation("Deleted ActivityConstraint {ActivityConstraintId} for Organization {OrganizationId}", activityConstraintId, organizationId);
    }

}