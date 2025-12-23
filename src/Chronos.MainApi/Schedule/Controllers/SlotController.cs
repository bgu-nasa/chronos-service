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
[Route("api/slots")]
public class SlotController(
    ISlotService slotService,
    ILogger<SlotController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSlotRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create slot endpoint was called for organization {OrganizationId}", organizationId);
        var id = await slotService.CreateSlotAsync(organizationId, request.SchedulingPeriodId, request.Weekday, request.FromTime, request.ToTime);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get slot endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        
        var result = await slotService.GetSlotAsync(organizationId, id);
        
        if (result == null)
            return NotFound();

        var response = new SlotResponse(result.Id.ToString(), result.SchedulingPeriodId.ToString(), result.Weekday, result.FromTime, result.ToTime);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all slots endpoint was called for organization {OrganizationId}", organizationId);
        var results = await slotService.GetAllSlotsAsync(organizationId);
        var response = results.Select(r => new SlotResponse(r.Id.ToString(), r.SchedulingPeriodId.ToString(), r.Weekday, r.FromTime, r.ToTime)).ToList();
        return Ok(response);
    }
    
    [HttpGet("scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetBySchedulingPeriod([FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get slots by scheduling period endpoint was called for organization {OrganizationId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, schedulingPeriodId);
        var results = await slotService.GetSlotsBySchedulingPeriodAsync(organizationId, schedulingPeriodId);
        var response = results.Select(r => new SlotResponse(r.Id.ToString(), r.SchedulingPeriodId.ToString(), r.Weekday, r.FromTime, r.ToTime)).ToList();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSlotRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update slot endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await slotService.UpdateSlotAsync(organizationId, id, request.Weekday, request.FromTime, request.ToTime);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete slot endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await slotService.DeleteSlotAsync(organizationId, id);
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
