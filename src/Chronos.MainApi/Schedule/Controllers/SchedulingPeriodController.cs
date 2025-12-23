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
[Route("api/scheduling-periods")]
public class SchedulingPeriodController(
    ISchedulingPeriodService schedulingPeriodService,
    ILogger<SchedulingPeriodController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSchedulingPeriodRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create scheduling period endpoint was called for organization {OrganizationId}", organizationId);
        var id = await schedulingPeriodService.CreateSchedulingPeriodAsync(organizationId, request.Name, request.FromDate, request.ToDate);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get scheduling period endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        
        var result = await schedulingPeriodService.GetSchedulingPeriodAsync(organizationId, id);
        
        if (result == null)
            return NotFound();

        var response = new SchedulingPeriodResponse(result.Id.ToString(), result.Name, result.FromDate, result.ToDate);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all scheduling periods endpoint was called for organization {OrganizationId}", organizationId);
        var results = await schedulingPeriodService.GetAllSchedulingPeriodsAsync(organizationId);
        var response = results.Select(r => new SchedulingPeriodResponse(r.Id.ToString(), r.Name, r.FromDate, r.ToDate)).ToList();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSchedulingPeriodRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update scheduling period endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await schedulingPeriodService.UpdateSchedulingPeriodAsync(organizationId, id, request.Name, request.FromDate, request.ToDate);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete scheduling period endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await schedulingPeriodService.DeleteSchedulingPeriodAsync(organizationId, id);
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
