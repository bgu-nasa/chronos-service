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
[Route("api/activity-constraints")]
public class ActivityConstraintController(
    IActivityConstraintService activityConstraintService,
    ILogger<ActivityConstraintController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActivityConstraintRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create activity constraint endpoint was called for organization {OrganizationId}", organizationId);
        var id = await activityConstraintService.CreateActivityConstraintAsync(organizationId, request.ActivityId, request.Key, request.Value);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get activity constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        
        var result = await activityConstraintService.GetActivityConstraintByIdAsync(organizationId, id);
        
        if (result == null)
            return NotFound();

        var response = new ActivityConstraintResponse(result.Id.ToString(), result.ActivityId.ToString(), result.OrganizationId.ToString(), result.Key, result.Value);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all activity constraints endpoint was called for organization {OrganizationId}", organizationId);
        var results = await activityConstraintService.GetAllActivityConstraintsAsync(organizationId);
        var response = results.Select(r => new ActivityConstraintResponse(r.Id.ToString(), r.ActivityId.ToString(), r.OrganizationId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }
    
    [HttpGet("activity/{activityId}")]
    public async Task<IActionResult> GetByActivity([FromRoute] Guid activityId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get activity constraints by activity endpoint was called for organization {OrganizationId} and activityId {ActivityId}", organizationId, activityId);
        var results = await activityConstraintService.GetByActivityIdAsync(organizationId, activityId);
        var response = results.Select(r => new ActivityConstraintResponse(r.Id.ToString(), r.ActivityId.ToString(), r.OrganizationId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateActivityConstraintRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update activity constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await activityConstraintService.UpdateActivityConstraintAsync(organizationId, id, request.Key, request.Value);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete activity constraint endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await activityConstraintService.DeleteActivityConstraintAsync(organizationId, id);
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
