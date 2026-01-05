using System.Security.Claims;
using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Middleware;
using Chronos.Shared.Exceptions;
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
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // TODO move to extension
        if (userIdClaim is null)
        {
            logger.LogError("No user id claim found in HttpContext, when expecting one");
            throw new UnauthorizedAccessException();
        }

        var isValidUserId = Guid.TryParse(userIdClaim.Value, out var userId);
        if (!isValidUserId)
        {
            logger.LogError("Invalid user id in claims: {UserId}", userIdClaim.Value);
            throw new UnauthorizedAccessException();
        }

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