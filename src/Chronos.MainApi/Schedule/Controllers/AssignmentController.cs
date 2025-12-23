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
[Route("api/assignments")]
public class AssignmentController(
    IAssignmentService assignmentService,
    ILogger<AssignmentController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create assignment endpoint was called for organization {OrganizationId}", organizationId);
        var id = await assignmentService.CreateAssignmentAsync(organizationId, request.SlotId, request.ResourceId, request.ScheduledItemId);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get assignment endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        
        var result = await assignmentService.GetAssignmentAsync(organizationId, id);
        
        if (result == null)
            return NotFound();

        var response = new AssignmentResponse(result.Id.ToString(), result.SlotId.ToString(), result.ResourceId.ToString(), result.ScheduledItemId.ToString());
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all assignments endpoint was called for organization {OrganizationId}", organizationId);
        var results = await assignmentService.GetAllAssignmentsAsync(organizationId);
        var response = results.Select(r => new AssignmentResponse(r.Id.ToString(), r.SlotId.ToString(), r.ResourceId.ToString(), r.ScheduledItemId.ToString())).ToList();
        return Ok(response);
    }
    
    [HttpGet("slot/{slotId}")]
    public async Task<IActionResult> GetBySlot([FromRoute] Guid slotId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get assignments by slot endpoint was called for organization {OrganizationId} and slotId {SlotId}", organizationId, slotId);
        var results = await assignmentService.GetAssignmentsBySlotAsync(organizationId, slotId);
        var response = results.Select(r => new AssignmentResponse(r.Id.ToString(), r.SlotId.ToString(), r.ResourceId.ToString(), r.ScheduledItemId.ToString())).ToList();
        return Ok(response);
    }

    [HttpGet("scheduled-item/{scheduledItemId}")]
    public async Task<IActionResult> GetByScheduledItem([FromRoute] Guid scheduledItemId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get assignments by scheduled item endpoint was called for organization {OrganizationId} and scheduledItemId {ScheduledItemId}", organizationId, scheduledItemId);
        var results = await assignmentService.GetAssignmentsByScheduledItemAsync(organizationId, scheduledItemId);
        var response = results.Select(r => new AssignmentResponse(r.Id.ToString(), r.SlotId.ToString(), r.ResourceId.ToString(), r.ScheduledItemId.ToString())).ToList();
        return Ok(response);
    }

    [HttpGet("slot/{slotId}/scheduled-item/{scheduledItemId}")]
    public async Task<IActionResult> GetBySlotAndScheduledItem([FromRoute] Guid slotId, [FromRoute] Guid scheduledItemId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get assignments by slot and scheduled item endpoint was called for organization {OrganizationId}, slotId {SlotId} and scheduledItemId {ScheduledItemId}", organizationId, slotId, scheduledItemId);
        var result = await assignmentService.GetAssignmentBySlotAndScheduledItemAsync(organizationId, slotId, scheduledItemId);
        
        if (result == null)
            return NotFound();

        var response = new AssignmentResponse(result.Id.ToString(), result.SlotId.ToString(), result.ResourceId.ToString(), result.ScheduledItemId.ToString());
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAssignmentRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update assignment endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await assignmentService.UpdateAssignmentAsync(organizationId, id, request.SlotId, request.ResourceId, request.ScheduledItemId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete assignment endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await assignmentService.DeleteAssignmentAsync(organizationId, id);
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
