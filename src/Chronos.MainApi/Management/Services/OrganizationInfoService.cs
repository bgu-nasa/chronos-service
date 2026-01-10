using Chronos.MainApi.Management.Contracts;
using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services.External;

namespace Chronos.MainApi.Management.Services;

public class OrganizationInfoService(ILogger<OrganizationInfoService> logger,
    IOrganizationService organizationService,
    IDepartmentService departmentService,
    IAuthClient authClient,
    IRoleService roleService)
    : IOrganizationInfoService
{
    public async Task<OrganizationInformation> GetOrganizationInformationAsync(Guid organizationId, Guid userId)
    {
        logger.LogInformation("Get organization information for organization {OrganizationId}", organizationId);

        var organization = await organizationService.GetOrganizationAsync(organizationId);
        var departments = await departmentService.GetDepartmentsAsync(organizationId);
        var departmentsResponse = departments.Select(d => d.ToDepartmentResponse());

        logger.LogInformation("Found {departmentsCount} departments for organization {OrganizationId}", departments.Count, organizationId);

        var roles = await roleService.GetUserAssignmentsAsync(organizationId, userId);
        var rolesResponse = roles.Select(r => new RoleAssignmentResponse(r.Id, r.UserId, r.OrganizationId, r.DepartmentId, r.Role));

        logger.LogInformation("Found {rolesCount} roles in organization {OrganizationId}", roles.Count, organizationId);

        var user = await authClient.GetUserAsync(organizationId, userId);
        var userFullName = $"{user.FirstName} {user.LastName}";

        return new OrganizationInformation(
            organization.Id,
            organization.Name,
            organization.Deleted,
            organization.DeletedTime,
            user.Email,
            userFullName,
            user.AvatarUrl,
            rolesResponse.ToArray(),
            departmentsResponse.ToArray()
        );
    }
}