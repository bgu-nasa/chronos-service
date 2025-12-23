using Chronos.Data.Repositories.Management;
using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Schedule.Services;

public class ScheduleValidationService(
    IOrganizationRepository organizationRepository,
    ISchedulingPeriodRepository schedulingPeriodRepository,
    ISlotRepository slotRepository,
    IAssignmentRepository assignmentRepository,
    IUserConstraintRepository userConstraintRepository,
    IUserPreferenceRepository userPreferenceRepository,
    IOrganizationPolicyRepository organizationPolicyRepository,
    IActivityConstraintRepository activityConstraintRepository,
    ILogger<ScheduleValidationService> logger)
{
    public async Task ValidateOrganizationAsync(Guid organizationId)
    {
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new NotFoundException("Organization not found");
        }
    }
    public async Task<SchedulingPeriod> ValidateAndGetSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        var period = await schedulingPeriodRepository.GetByIdAsync(schedulingPeriodId);

        if (period == null || period.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "SchedulingPeriod not found or does not belong to organization. SchedulingPeriodId: {SchedulingPeriodId}, OrganizationId: {OrganizationId}",
                schedulingPeriodId, organizationId);

            throw new NotFoundException("Scheduling period not found");
        }

        return period;
    }

    public async Task<Slot> ValidateAndGetSlotAsync(Guid organizationId, Guid slotId)
    {
        var slot = await slotRepository.GetByIdAsync(slotId);
        if (slot == null || slot.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "Slot not found or does not belong to organization. SlotId: {SlotId}, OrganizationId: {OrganizationId}",
                slotId, organizationId);
            throw new NotFoundException("Slot not found");
        }
        return slot;
    }
    
    public async Task<Assignment> ValidateAndGetAssignmentAsync(Guid organizationId, Guid assignmentId)
    {
        var assignment = await assignmentRepository.GetByIdAsync(assignmentId);
        if (assignment == null || assignment.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "Assignment not found or does not belong to organization. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
                assignmentId, organizationId);
            throw new NotFoundException("Assignment not found");
        }
        return assignment;
    }
    
    public async Task<UserConstraint> ValidateAndGetUserConstraintAsync(Guid organizationId, Guid userConstraintId)
    {
        var constraint = await userConstraintRepository.GetByIdAsync(userConstraintId);
        if (constraint == null || constraint.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "UserConstraint not found or does not belong to organization. UserConstraintId: {UserConstraintId}, OrganizationId: {OrganizationId}",
                userConstraintId, organizationId);
            throw new NotFoundException("User constraint not found");
        }
        return constraint;
    }
    
    public async Task<UserPreference> ValidateAndGetUserPreferenceAsync(Guid organizationId, Guid userPreferenceId)
    {
        var preference = await userPreferenceRepository.GetByIdAsync(userPreferenceId);
        if (preference == null || preference.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "UserPreference not found or does not belong to organization. UserPreferenceId: {UserPreferenceId}, OrganizationId: {OrganizationId}",
                userPreferenceId, organizationId);
            throw new NotFoundException("User preference not found");
        }
        return preference;
    }
    
    public async Task<OrganizationPolicy> ValidateAndGetOrganizationPolicyAsync(Guid organizationId, Guid organizationPolicyId)
    {
        var policy = await organizationPolicyRepository.GetByIdAsync(organizationPolicyId);
        if (policy == null || policy.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "OrganizationPolicy not found or does not belong to organization. OrganizationPolicyId: {OrganizationPolicyId}, OrganizationId: {OrganizationId}",
                organizationPolicyId, organizationId);
            throw new NotFoundException("Organization policy not found");
        }
        return policy;
    }
    
    public async Task<ActivityConstraint> ValidateAndGetActivityConstraintAsync(Guid organizationId, Guid activityConstraintId)
    {
        var constraint = await activityConstraintRepository.GetByIdAsync(activityConstraintId);
        if (constraint == null || constraint.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "ActivityConstraint not found or does not belong to organization. ActivityConstraintId: {ActivityConstraintId}, OrganizationId: {OrganizationId}",
                activityConstraintId, organizationId);
            throw new NotFoundException("Activity constraint not found");
        }
        return constraint;
    }
}
