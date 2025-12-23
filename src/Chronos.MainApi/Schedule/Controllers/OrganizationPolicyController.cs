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
[Route("api/organization-policies")]
public class OrganizationPolicyController(
    IOrganizationPolicyService organizationPolicyService,
    ILogger<OrganizationPolicyController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationPolicyRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Create organization policy endpoint was called for organization {OrganizationId}", organizationId);
        var result = await organizationPolicyService.CreatePolicyAsync(organizationId, request.SchedulingPeriodId, request.Key, request.Value);
        return CreatedAtAction(nameof(GetAll), new { }, new { id = result.Id }); // There is no Get(id) in the service interface I saw earlier, only GetAll or GetBySchedulingPeriod. Let's re-check the service.
        /* 
         * IOrganizationPolicyService has:
         * CreatePolicyAsync
         * GetAllPoliciesAsync
         * GetPoliciesBySchedulingPeriodIdsAsync
         * UpdatePolicyAsync
         * DeletePolicyAsync
         * 
         * It seems missing GetPolicyById. I will use GetAll for CreatedAtAction or just return Ok(response).
         * Actually, standard pattern is return Created. I'll stick to returning the created object.
         */
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get all organization policies endpoint was called for organization {OrganizationId}", organizationId);
        var results = await organizationPolicyService.GetAllPoliciesAsync(organizationId);
        var response = results.Select(r => new OrganizationPolicyResponse(r.Id.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }
    
    [HttpGet("scheduling-period/{schedulingPeriodId}")]
    public async Task<IActionResult> GetBySchedulingPeriod([FromRoute] Guid schedulingPeriodId)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Get organization policies by scheduling period endpoint was called for organization {OrganizationId} and schedulingPeriodId {SchedulingPeriodId}", organizationId, schedulingPeriodId);
        var results = await organizationPolicyService.GetPoliciesBySchedulingPeriodIdsAsync(organizationId, schedulingPeriodId);
        var response = results.Select(r => new OrganizationPolicyResponse(r.Id.ToString(), r.OrganizationId.ToString(), r.SchedulingPeriodId.ToString(), r.Key, r.Value)).ToList();
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOrganizationPolicyRequest request)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Update organization policy endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await organizationPolicyService.UpdatePolicyAsync(organizationId, id, request.Key, request.Value);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var organizationId = GetOrganizationIdFromContext();
        logger.LogInformation("Delete organization policy endpoint was called for organization {OrganizationId} and id {Id}", organizationId, id);
        await organizationPolicyService.DeletePolicyAsync(organizationId, id);
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
