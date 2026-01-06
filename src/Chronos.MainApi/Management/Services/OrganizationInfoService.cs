using Chronos.MainApi.Management.Contracts;
using Chronos.MainApi.Management.Extensions;

namespace Chronos.MainApi.Management.Services;

public class OrganizationInfoService(ILogger<OrganizationInfoService> logger,
    IOrganizationService organizationService,
    IDepartmentService departmentService,
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
        var rolesResponse = roles.Select(r => new RoleAssignmentResponse(r.OrganizationId, r.DepartmentId, r.Role.ToRoleType()));

        logger.LogInformation("Found {rolesCount} roles in organization {OrganizationId}", roles.Count, organizationId);

        // TODO Mapper extension for departments and roles

        return new OrganizationInformation(
            organization.Id,
            organization.Name,
            organization.Deleted,
            organization.DeletedTime,
            rolesResponse.ToArray(),
            departmentsResponse.ToArray()
        );
    }
}