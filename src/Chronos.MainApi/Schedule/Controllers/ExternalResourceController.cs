using Chronos.MainApi.Schedule.Extensions;
using Chronos.MainApi.Schedule.Services;
using Chronos.MainApi.Shared.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Schedule.Controllers;

[ApiController]
[Route("api/external-resources/[controller]")]
public class ExternalResourceController(
    ILogger<ExternalResourceController> logger,
    IExternalResourceService resourceService
    ) : ControllerBase
{
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet]
    public async Task<IActionResult> GetResourcesAsync()
    {
        logger.LogInformation("Get resources endpoint was called.");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var resources = await resourceService.GetResourcesAsync(organizationId);

        var resourceResponses = resources.Select(r => r.ToResourceResponse()).ToList();
        
        return Ok(resourceResponses);
    }
    
}