using System.Security.Claims;
using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Middleware;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
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

        var organizationId = GetOrganizationIdFromContext();
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

        var orgInfo = await infoService.GetOrganizationInformationAsync(Guid.Parse(organizationId), userId);

        return Ok(orgInfo);
    }

    [Authorize(Policy = "OrgRole:Administrator")]
    [HttpDelete]
    public async Task<IActionResult> DeleteOrganization()
    {
        logger.LogInformation("Soft delete organization endpoint was called");

        var organizationId = GetOrganizationIdFromContext();

        try
        {
            await organizationService.SetForDeletionAsync(Guid.Parse(organizationId));

            return NoContent();
        }
        catch (FormatException e)
        {
            logger.LogInformation("The organization ID provided is not in a valid format: {OrganizationId}", organizationId);
            throw new BadRequestException("The organization ID is not in a valid format.", e);
        }
    }

    [Authorize(Policy = "OrgRole:Administrator")]
    [HttpPost("/restore")]
    public async Task<IActionResult> RestoreOrganization()
    {
        logger.LogInformation("Restore organization endpoint was called");

        var organizationId = GetOrganizationIdFromContext();

        try
        {
            await organizationService.RestoreDeletedOrganizationAsync(Guid.Parse(organizationId));

            return NoContent();
        }
        catch (FormatException e)
        {
            logger.LogInformation("The organization ID provided is not in a valid format: {OrganizationId}", organizationId);
            throw new BadRequestException("The organization ID is not in a valid format.", e);
        }
    }

    private string GetOrganizationIdFromContext()
    {
        var organizationId = HttpContext.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return organizationId;
    }
}