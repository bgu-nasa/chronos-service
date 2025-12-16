using Chronos.Data.Repositories.Management;
using Chronos.Domain.Management.Roles;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Management.Services;

public class RoleService(
    IRoleAssignmentRepository roleAssignmentRepository,
    ManagementValidationService validationService,
    ILogger<RoleService> logger) : IRoleService
{
    public async Task<List<RoleAssignment>> GetAllAssignments(Guid organizationId)
    {
        logger.LogDebug("Retrieving all role assignments for organization. OrganizationId: {OrganizationId}", organizationId);
        
        await validationService.ValidateOrganizationAsync(organizationId);

        var assignments = await roleAssignmentRepository.GetAllAsync(organizationId);
        
        logger.LogDebug("Retrieved {Count} role assignments for organization. OrganizationId: {OrganizationId}", assignments.Count, organizationId);
        return assignments;
    }

    public async Task<List<RoleAssignment>> GetUserAssignments(Guid organizationId, Guid userId)
    {
        logger.LogDebug("Retrieving role assignments for user. OrganizationId: {OrganizationId}, UserId: {UserId}", organizationId, userId);
        
        await validationService.ValidateOrganizationAsync(organizationId);

        var assignments = await roleAssignmentRepository.GetUserAssignmentsAsync(organizationId, userId);
        
        logger.LogDebug("Retrieved {Count} role assignments for user. OrganizationId: {OrganizationId}, UserId: {UserId}", assignments.Count, organizationId, userId);
        return assignments;
    }

    public async Task<RoleAssignment> GetAssignment(Guid organizationId, Guid roleAssignmentId)
    {
        logger.LogDebug("Retrieving role assignment. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);
        
        await validationService.ValidateOrganizationAsync(organizationId);

        var assignment = await roleAssignmentRepository.GetAsync(organizationId, roleAssignmentId);

        if (assignment == null)
        {
            logger.LogWarning("Role assignment not found. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);
            throw new NotFoundException("Role assignment not found");
        }

        return assignment;
    }

    public async Task<RoleAssignment> AddAssignment(Guid organizationId, Guid? departmentId, Guid userId, Role role)
    {
        logger.LogInformation("Adding role assignment. OrganizationId: {OrganizationId}, DepartmentId: {DepartmentId}, UserId: {UserId}, Role: {Role}",
            organizationId, departmentId, userId, role);
        
        await validationService.ValidateOrganizationAsync(organizationId);

        if (departmentId.HasValue)
        {
            await validationService.ValidateAndGetDepartmentAsync(organizationId, departmentId.Value, excludeDeleted: true);
        }

        var roleAssignment = new RoleAssignment
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            DepartmentId = departmentId,
            UserId = userId,
            Role = role
        };

        var addedAssignment = await roleAssignmentRepository.AddAsync(roleAssignment);
        
        logger.LogInformation("Role assignment added successfully. RoleAssignmentId: {RoleAssignmentId}, OrganizationId: {OrganizationId}, UserId: {UserId}", 
            addedAssignment.Id, organizationId, userId);
        
        return addedAssignment;
    }

    public async Task RemoveAssignment(Guid organizationId, Guid roleAssignmentId)
    {
        logger.LogInformation("Removing role assignment. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);
        
        await validationService.ValidateOrganizationAsync(organizationId);

        var assignment = await roleAssignmentRepository.GetAsync(organizationId, roleAssignmentId);

        if (assignment == null)
        {
            logger.LogWarning("Role assignment not found. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);
            throw new NotFoundException("Role assignment not found");
        }

        await roleAssignmentRepository.DeleteAsync(organizationId, roleAssignmentId);
        
        logger.LogInformation("Role assignment removed successfully. RoleAssignmentId: {RoleAssignmentId}, OrganizationId: {OrganizationId}", roleAssignmentId, organizationId);
    }

}