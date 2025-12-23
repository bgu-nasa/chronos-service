using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Schedule.Contracts;
using Chronos.MainApi.Schedule.Services;
using Chronos.MainApi.Shared.Middleware;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Schedule.Controllers;

[ApiController]
[Authorize]
[RequireOrganization]
[Route("api/user-constraints")]
public class UserConstraintController(
    IUserConstraintService userConstraintService,
    ILogger<UserConstraintController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserConstraintRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create user constraint endpoint was called for organization {OrganizationId}", organizationId);
        var id = await userConstraintService.CreateUserConstraintAsync(organizationId, request.UserId, request.SchedulingPeriodId, request.Key, request.Value);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        
        var result = await userConstraintService.GetUserConstraintByIdAsync(organizationId, id);
        
        if (result == null)
            return NotFound();

        var response = new UserConstraintResponse(result.Id.ToString(), result.UserId.ToString(), result.OrganizationId.ToString(), result.SchedulingPeriodId.ToString(), result.Key, result.Value);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all user constraints endpoint was called for organization {OrganizationId}", organizationId);
        var results = await userConstraintService.GetAllUserConstraintsAsync(organizationId);
        var response = results.Select(r => new UserConstraintResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user constraints by user endpoint was called for organization {OrganizationId} and userId {UserId}", organizationId, userId);
        var results = await userConstraintService.GetByUserIdAsync(organizationId, userId);
        var response = results.Select(r => new UserConstraintResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpGet("scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetBySchedulingPeriod([FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user constraints by scheduling period endpoint was called for organization {OrganizationId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, schedulingPeriodId);
        var results = await userConstraintService.GetBySchedulingPeriodIdAsync(organizationId, schedulingPeriodId);
        var response = results.Select(r => new UserConstraintResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpGet("user/{userId}/scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetByUserAndSchedulingPeriod([FromRoute] Guid userId, [FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user constraints by user and scheduling period endpoint was called for organization {OrganizationId}, userId {UserId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, userId, schedulingPeriodId);
        var results = await userConstraintService.GetBySchedulingPeriodAndUserIdAsync(organizationId, schedulingPeriodId, userId);
        var response = results.Select(r => new UserConstraintResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserConstraintRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update user constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await userConstraintService.UpdateUserConstraintAsync(organizationId, id, request.Key, request.Value);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete user constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await userConstraintService.DeleteUserConstraintAsync(organizationId, id);
        return NoContent();
    }

    private Guid GetOrganizationIdFromContext()
    {
        var organizationId = HttpContext.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return Guid.Parse(organizationId);
    }
}
