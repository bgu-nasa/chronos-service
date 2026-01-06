using Chronos.MainApi.Management.Contracts;
using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Management.Controllers;

[ApiController]
[RequireOrganization]
[Route("api/management/[controller]")]
public class RoleController(
    ILogger<RoleController> logger,
    IRoleService roleService)
: ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllRoleAssignments()
    {
        logger.LogInformation("Get all role assignments");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        var assignments = await roleService.GetAllAssignmentsAsync(organizationId);
        return Ok(assignments.Select(a => a.ToRoleAssignmentResponse()).ToArray());
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRoleAssignments(string userId)
    {
        logger.LogInformation("Get role assignments for user: {UserId}", userId);
        
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        if (!Guid.TryParse(userId, out var userGuid))
        {
            logger.LogInformation("Invalid format of user ID in request.");
            return BadRequest("Invalid format of user ID in request.");
        }
        
        var assignments = await roleService.GetUserAssignmentsAsync(organizationId, userGuid);
        return Ok(assignments.Select(a => a.ToRoleAssignmentResponse()).ToArray());
    }

    [Authorize]
    [HttpGet("{roleAssignmentId}")]
    public async Task<IActionResult> GetRoleAssignmentById(string roleAssignmentId)
    {
        logger.LogInformation("Get role assignment by id: {RoleAssignmentId}", roleAssignmentId);
        
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        if (!Guid.TryParse(roleAssignmentId, out var assignmentGuid))
        {
            logger.LogInformation("Invalid format of role assignment ID in request.");
            return BadRequest("Invalid format of role assignment ID in request.");
        }
        
        var assignment = await roleService.GetAssignmentAsync(organizationId, assignmentGuid);
        
        return Ok(assignment.ToRoleAssignmentResponse());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddRoleAssignment([FromBody] RoleAssignmentRequest request)
    {
        logger.LogInformation("Add role assignment for user: {UserId}, Role: {Role}, DepartmentId: {DepartmentId}",
            request.UserId, request.Role, request.DepartmentId);
        
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var assignment = await roleService.AddAssignmentAsync(
            organizationId,
            request.DepartmentId,
            request.UserId,
            request.Role.ToDomainRole());
        
        return CreatedAtAction(
            nameof(GetRoleAssignmentById),
            new { roleAssignmentId = assignment.Id },
            assignment.ToRoleAssignmentResponse()
        );
    }

    [Authorize]
    [HttpDelete("{roleAssignmentId}")]
    public async Task<IActionResult> RemoveRoleAssignment([FromRoute] string roleAssignmentId)
    {
        logger.LogInformation("Remove role assignment with id: {RoleAssignmentId}", roleAssignmentId);
        
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        if (!Guid.TryParse(roleAssignmentId, out var assignmentGuid))
        {
            logger.LogInformation("Invalid format of role assignment ID in request.");
            return BadRequest("Invalid format of role assignment ID in request.");
        }
        
        await roleService.RemoveAssignmentAsync(organizationId, assignmentGuid);
        
        return NoContent();
    }
}
