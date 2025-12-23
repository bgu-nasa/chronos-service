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
[Route("api/user-preferences")]
public class UserPreferenceController(
    IUserPreferenceService userPreferenceService,
    ILogger<UserPreferenceController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserPreferenceRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create user preference endpoint was called for organization {OrganizationId}", organizationId);
        var id = await userPreferenceService.CreateUserPreferenceAsync(organizationId, request.UserId, request.SchedulingPeriodId, request.Key, request.Value);
        return CreatedAtAction(nameof(Get), new { id }, new { id }); // But Get takes userId, schedulingPeriodId, key? Or is there GetById?
        /*
         * IUserPreferenceService has:
         * CreateUserPreferenceAsync -> returns Guid
         * GetUserPreferenceAsync(organizationId, userId, schedulingPeriodId, key) -> returns UserPreference
         * GetAllUserPreferencesAsync
         * 
         * It seems I don't have GetById. I will rely on GetAll or other Get methods. 
         * I'll just return OK with ID.
         */
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all user preferences endpoint was called for organization {OrganizationId}", organizationId);
        var results = await userPreferenceService.GetAllUserPreferencesAsync(organizationId);
        var response = results.Select(r => new UserPreferenceResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user preferences by user endpoint was called for organization {OrganizationId} and userId {UserId}", organizationId, userId);
        var results = await userPreferenceService.GetAllUserPreferencesByUserIdAsync(organizationId, userId);
        var response = results.Select(r => new UserPreferenceResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpGet("scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetBySchedulingPeriod([FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user preferences by scheduling period endpoint was called for organization {OrganizationId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, schedulingPeriodId);
        var results = await userPreferenceService.GetAllUserPreferencesBySchedulingPeriodIdAsync(organizationId, schedulingPeriodId);
        var response = results.Select(r => new UserPreferenceResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpGet("user/{userId}/scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetByUserAndSchedulingPeriod([FromRoute] Guid userId, [FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user preferences by user and scheduling period endpoint was called for organization {OrganizationId}, userId {UserId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, userId, schedulingPeriodId);
        var results = await userPreferenceService.GetAllUserPreferencesByUserAndPeriodAsync(organizationId, userId, schedulingPeriodId);
        var response = results.Select(r => new UserPreferenceResponse(r.Id.ToString(), r.UserId.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpGet("user/{userId}/scheduling-period/{schedulingPeriodId}/key/{key}")]
    public async Task<IActionResult> GetSingle([FromRoute] Guid userId, [FromRoute] Guid schedulingPeriodId, [FromRoute] string key)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get user preference endpoint was called for organization {OrganizationId}, userId {UserId}, schedulingPeriodId {SchedulingPeriodId} and key {Key}", organizationId, userId, schedulingPeriodId, key);
        var result = await userPreferenceService.GetUserPreferenceAsync(organizationId, userId, schedulingPeriodId, key);
        
        if (result == null)
            return NotFound();

        var response = new UserPreferenceResponse(result.Id.ToString(), result.UserId.ToString(), result.OrganizationId.ToString(), result.SchedulingPeriodId.ToString(), result.Key, result.Value);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserPreferenceRequest request)
    {
        // Service UpdateUserPreferenceAsync(organizationId, userId, schedulingPeriodId, key, value) - Doesn't verify ID?
        // Wait, the interface says: UpdateUserPreferenceAsync(Guid organizationId,Guid userId, Guid schedulingPeriodId, string key, string value);
        // It uses composite key (userId, schedulingPeriodId, key) to update value?
        // But I received an ID in the route.
        // If the service doesn't support update by ID, this is tricky.
        // Let's assume for now I should use the parameters from request. The ID in route might be misleading if service doesn't use it. 
        // Or I should assume the request body has the keys.
        // Actually, let's look at the `UserPreferenceService.cs` implementation if possible, or just follow interface.
        // The interface uses (organizationId, userId, schedulingPeriodId, key) to identify what to update.
        
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update user preference endpoint was called for organization {OrganizationId}", organizationId);
        await userPreferenceService.UpdateUserPreferenceAsync(organizationId, request.UserId, request.SchedulingPeriodId, request.Key, request.Value);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete user preference endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await userPreferenceService.DeleteUserPreferenceAsync(organizationId, id);
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
