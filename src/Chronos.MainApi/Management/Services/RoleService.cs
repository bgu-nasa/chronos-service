using Chronos.Data.Repositories.Management;
using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Management.Contracts;
using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services.External;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Management.Services;

public class RoleService(
    IRoleAssignmentRepository roleAssignmentRepository,
    IAuthClient authClient,
    ManagementValidationService validationService,
    ILogger<RoleService> logger) : IRoleService
{
    public async Task<List<RoleAssignmentResponse>> GetAllAssignmentsAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all role assignments for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var assignments = await roleAssignmentRepository.GetAllAsync(organizationId);

        logger.LogDebug("Retrieved {Count} role assignments for organization. OrganizationId: {OrganizationId}", assignments.Count, organizationId);
        return assignments.Select(a => a.ToRoleAssignmentResponse()).ToList();
    }

    public async Task<List<RoleAssignmentResponse>> GetUserAssignmentsAsync(Guid organizationId, Guid userId)
    {
        logger.LogDebug("Retrieving role assignments for user. OrganizationId: {OrganizationId}, UserId: {UserId}", organizationId, userId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var assignments = await roleAssignmentRepository.GetUserAssignmentsAsync(organizationId, userId);

        logger.LogDebug("Retrieved {Count} role assignments for user. OrganizationId: {OrganizationId}, UserId: {UserId}", assignments.Count, organizationId, userId);
        return assignments.Select(a => a.ToRoleAssignmentResponse()).ToList();
    }

    public async Task<RoleAssignmentResponse> GetAssignmentAsync(Guid organizationId, Guid roleAssignmentId)
    {
        logger.LogDebug("Retrieving role assignment. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var assignment = await roleAssignmentRepository.GetAsync(organizationId, roleAssignmentId);

        if (assignment == null)
        {
            logger.LogWarning("Role assignment not found. OrganizationId: {OrganizationId}, RoleAssignmentId: {RoleAssignmentId}", organizationId, roleAssignmentId);
            throw new NotFoundException("Role assignment not found");
        }

        return assignment.ToRoleAssignmentResponse();
    }

    public async Task<RoleAssignmentResponse> AddAssignmentAsync(Guid organizationId, Guid? departmentId, Guid userId, Role role)
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

        return addedAssignment.ToRoleAssignmentResponse();
    }

    public async Task RemoveAssignmentAsync(Guid organizationId, Guid roleAssignmentId)
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

    public async Task<List<UserRoleAssignmentSummary>> GetRoleAssignmentsSummaryAsync(Guid organizationId)
    {
        logger.LogInformation("Retrieving role assignments summary for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidateOrganizationAsync(organizationId);

        // Fetch all assignments for the organization
        var assignments = await roleAssignmentRepository.GetAllAsync(organizationId);

        if (assignments.Count == 0)
        {
            logger.LogInformation("No role assignments found for organization. OrganizationId: {OrganizationId}", organizationId);
            return new List<UserRoleAssignmentSummary>();
        }

        // Fetch all users in one call for efficient mapping
        var users = await authClient.GetUsersAsync(organizationId);
        var userEmailMap = users.ToDictionary(u => Guid.Parse(u.Id), u => u.Email);

        // Group assignments by user and convert to responses efficiently
        var summary = assignments
            .GroupBy(a => a.UserId)
            .Select(g => new UserRoleAssignmentSummary(
                UserEmail: userEmailMap.TryGetValue(g.Key, out var email) ? email : "Unknown",
                Assignments: g.Select(a => a.ToRoleAssignmentResponse()).ToArray()
            ))
            .OrderBy(s => s.UserEmail)
            .Where(s => s.UserEmail != "Unknown")
            .Where(s => s.Assignments.Length != 0)
            .ToList();

        logger.LogInformation("Retrieved role assignments summary for {UserCount} users. OrganizationId: {OrganizationId}", summary.Count, organizationId);
        return summary;
    }

}