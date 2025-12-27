using Chronos.Data.Repositories.Management;
using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Resources;
using Chronos.MainApi.Management.Services;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Resources.Services;

public class ResourceValidationService(
    IOrganizationService organizationService,
    ISubjectRepository subjectRepository,
    IActivityRepository activityRepository,
    ILogger<ResourceValidationService> logger)
{
    public async Task ValidationOrganizationAsync(Guid organizationId)
    {
        var organization = await organizationService.GetOrganizationAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new NotFoundException("Organization not found");
        }
    }

    public async Task<Subject> ValidateAndGetSubjectAsync(Guid organizationId, Guid subjectId)
    {
        var subject = await subjectRepository.GetByIdAsync(subjectId);

        if (subject == null || subject.OrganizationId != organizationId)
        {
            logger.LogWarning("Subject not found or does not belong to organization. SubjectId: {SubjectId}, OrganizationId: {OrganizationId}", subjectId, organizationId);
            throw new NotFoundException("Subject not found");
        }

        return subject;
    }

    public async Task<Activity> ValidateAndGetActivityAsync(Guid organizationId, Guid activityId)
    {
        var activity = await activityRepository.GetByIdAsync(activityId);

        if (activity == null || activity.OrganizationId != organizationId)
        {
            logger.LogWarning("Activity not found or does not belong to organization. ActivityId: {ActivityId}, OrganizationId: {OrganizationId}", activityId, organizationId);
            throw new NotFoundException("Activity not found");
        }

        return activity;
    }
}