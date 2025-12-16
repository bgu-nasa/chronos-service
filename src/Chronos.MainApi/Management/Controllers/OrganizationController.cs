using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Middleware;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Management.Controllers;

[ApiController]
[Route("api/management/[controller]")]
public class OrganizationController(ILogger<OrganizationController> logger, IOrganizationService organizationService) : ControllerBase
{
    // TODO have the OrganizationInformation GET endpoint, nothing else for now.

    [RequireOrganization]
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

    [RequireOrganization]
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