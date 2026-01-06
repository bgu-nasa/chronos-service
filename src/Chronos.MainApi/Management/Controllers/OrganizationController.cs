using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Controllers.ControllerUtils;
using Chronos.MainApi.Shared.Extensions;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Management.Controllers;

[ApiController]
[RequireOrganization]
[Route("api/management/[controller]")]
public class OrganizationController(
    ILogger<OrganizationController> logger,
    IOrganizationService organizationService,
    IOrganizationInfoService infoService)
: ControllerBase
{
    [Authorize]
    [HttpGet("/info")]
    public async Task<IActionResult> GetOrganizationInfoAsync()
    {
        logger.LogInformation("Get organization info");

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        var userId = HttpContext.User.GetUserId();

        var orgInfo = await infoService.GetOrganizationInformationAsync(organizationId, userId);

        return Ok(orgInfo);
    }

    [Authorize(Policy = "OrgRole:Administrator")]
    [HttpDelete]
    public async Task<IActionResult> DeleteOrganization()
    {
        logger.LogInformation("Soft delete organization endpoint was called");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        await organizationService.SetForDeletionAsync(organizationId);
        return NoContent();
    }

    [Authorize(Policy = "OrgRole:Administrator")]
    [HttpPost("/restore")]
    public async Task<IActionResult> RestoreOrganization()
    {
        logger.LogInformation("Restore organization endpoint was called");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        await organizationService.RestoreDeletedOrganizationAsync(organizationId);
        return NoContent();
    }
}