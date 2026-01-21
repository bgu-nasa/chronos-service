using Chronos.Domain.Auth;
using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Management.Services;

namespace Chronos.MainApi.Auth.Services;

public class OnboardingService(ILogger<OnboardingService> logger, IOrganizationService organizationService, IRoleService roleService) : IOnboardingService
{
    public async Task<Guid> CreateOrganizationAsync(string organizationName, string plan)
    {
        logger.LogInformation("Onboarding started: creating organization {OrganizationName}", organizationName);
        logger.LogWarning("Plan {Plan} is ignored.", plan);

        return await organizationService.CreateOrganizationAsync(organizationName);
    }

    public async Task OnboardAdminUserAsync(Guid organizationId, User admin)
    {
        logger.LogInformation("Onboarding admin user for organization {OrganizationId}", admin.OrganizationId);

        var roleAssignment = await roleService.AddAssignmentAsync(organizationId, null, admin.Id, Role.Administrator, isSystemAssigned: true);

        logger.LogInformation("Assigned admin to organization {OrganizationId}, role id: {roleId}. This should conclude the onboarding process", organizationId, roleAssignment.Id);
    }
}