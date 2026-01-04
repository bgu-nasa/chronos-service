using Chronos.MainApi.Resources.Contracts;
using Chronos.MainApi.Resources.Services;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Resources.Controllers;

[ApiController]
[Route("api/resources/[controller]")]
public class ResourceController(
    ILogger<ResourceController> logger,
    IResourceService resourceService,
    IResourceTypeService resourceTypeService,
    IResourceAttributeService resourceAttributeService,
    IResourceAttributeAssignmentService resourceAttributeAssignmentService
    ) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateResourceAsync([FromBody] CreateResourceRequest request)
    {
        logger.LogInformation("Create resource endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var resourceId = await resourceService.CreateResourceAsync(
            request.Id,
            request.OrganizationId,
            request.ResourceTypeId,
            request.Location,
            request.Identifier,
            request.Capacity);
        
        return CreatedAtAction(nameof(GetResource), new { resourceId }, new { id = resourceId });
    }
    
    [Authorize]
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetResource([FromRoute] Guid resourceId)
    {
        logger.LogInformation("Get resource endpoint was called for resource {ResourceId}", resourceId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resource = await resourceService.GetResourceAsync(organizationId, resourceId);
        if (resource == null)
            return NotFound();

        var resourceResponse = new ResourceResponse(resourceId);
        
        return Ok(resourceResponse);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetResourcesAsync()
    {
        logger.LogInformation("Get resources endpoint was called.");
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resources = await resourceService.GetResourcesAsync(organizationId);

        var resourceResponses = resources.Select(r => new ResourceResponse(r.Id)).ToList();
        
        return Ok(resourceResponses);
    }
    
    [Authorize]
    [HttpPatch("{resourceId}")]
    public async Task<IActionResult> UpdateResourceAsync(Guid resourceId, [FromBody] UpdateResourceRequest request)
    {
        logger.LogInformation("Update resource endpoint was called for resource {ResourceId}", resourceId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceService.UpdateResourceAsync(
            organizationId,
            resourceId,
            request.ResourceTypeId,
            request.Location,
            request.Identifier,
            request.Capacity);

        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> DeleteResourceAsync(Guid resourceId)
    {
        logger.LogInformation("Delete resource endpoint was called for resource {ResourceId}", resourceId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceService.DeleteResourceAsync(organizationId, resourceId);

        return NoContent();
    }
    
    [Authorize]
    [HttpPost("types")]
    public async Task<IActionResult> CreateResourceTypeAsync([FromBody] CreateResourceTypeRequest request)
    {
        logger.LogInformation("Create resource type endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var resourceTypeId = await resourceTypeService.CreateResourceTypeAsync(
            request.OrganizationId,
            request.Type);
        
        return CreatedAtAction(nameof(GetResourceType), new { resourceTypeId }, new { id = resourceTypeId });
    }
    
    [Authorize]
    [HttpGet("types/{resourceTypeId}")]
    public async Task<IActionResult> GetResourceType(Guid resourceTypeId)
    {
        logger.LogInformation("Get resource type endpoint was called for resource type {ResourceTypeId}", resourceTypeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceType = await resourceTypeService.GetResourceTypeAsync(organizationId, resourceTypeId);
        if (resourceType == null)
            return NotFound();

        var resourceTypeResponse = new ResourceTypeResponse(resourceTypeId);
        
        return Ok(resourceTypeResponse);
    }
    
    [Authorize]
    [HttpGet("/types")]
    public async Task<IActionResult> GetResourceTypesAsync()
    {
        logger.LogInformation("Get resource types endpoint was called.");
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceTypes = await resourceTypeService.GetResourceTypesAsync(organizationId);

        var resourceTypeResponses = resourceTypes.Select(rt => new ResourceTypeResponse(rt.Id)).ToList();
        
        return Ok(resourceTypeResponses);
    }
    
    [Authorize]
    [HttpPatch("types/{resourceTypeId}")]
    public async Task<IActionResult> UpdateResourceTypeAsync(Guid resourceTypeId, [FromBody] UpdateResourceTypeRequest request)
    {
        logger.LogInformation("Update resource type endpoint was called for resource type {ResourceTypeId}", resourceTypeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceTypeService.UpdateResourceTypeAsync(
            organizationId,
            resourceTypeId,
            request.Type);

        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("types/{resourceTypeId}")]
    public async Task<IActionResult> DeleteResourceTypeAsync(Guid resourceTypeId)
    {
        logger.LogInformation("Delete resource type endpoint was called for resource type {ResourceTypeId}", resourceTypeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceTypeService.DeleteResourceTypeAsync(organizationId, resourceTypeId);

        return NoContent();
    }
    
    [Authorize]
    [HttpPost("/attributes")]
    public async Task<IActionResult> CreateResourceAttributeAsync(Guid resourceId, [FromBody] CreateResourceAttributeRequest request)
    {
        logger.LogInformation("Create resource attribute endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var resourceAttributeId = await resourceAttributeService.CreateResourceAttributeAsync(
            request.OrganizationId,
            request.Title,
            request.Description);
        
        return CreatedAtAction(nameof(GetResourceAttribute), new { resourceId, resourceAttributeId }, new { id = resourceAttributeId });
    }
    
    [Authorize]
    [HttpGet("/attributes/{resourceAttributeId}")]
    public async Task<IActionResult> GetResourceAttribute(Guid resourceAttributeId)
    {
        logger.LogInformation("Get resource attribute endpoint was called for resource attribute {ResourceAttributeId}", resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceAttribute = await resourceAttributeService.GetResourceAttributeAsync(organizationId, resourceAttributeId);
        if (resourceAttribute == null)
            return NotFound();

        var resourceAttributeResponse = new ResourceAttributeResponse(resourceAttributeId);
        
        return Ok(resourceAttributeResponse);
    }
    
    [Authorize]
    [HttpGet("/attributes")]
    public async Task<IActionResult> GetResourceAttributesAsync()
    {
        logger.LogInformation("Get resource attributes endpoint was called.");
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceAttributes = await resourceAttributeService.GetResourceAttributesAsync(organizationId);

        var resourceAttributeResponses = resourceAttributes.Select(ra => new ResourceAttributeResponse(ra.Id)).ToList();
        
        return Ok(resourceAttributeResponses);
    }
    
    [Authorize]
    [HttpPatch("/attributes/{resourceAttributeId}")]
    public async Task<IActionResult> UpdateResourceAttributeAsync(Guid resourceId, Guid resourceAttributeId, [FromBody] UpdateResourceAttributeRequest request)
    {
        logger.LogInformation("Update resource attribute endpoint was called for resource attribute {ResourceAttributeId}", resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceAttributeService.UpdateResourceAttributeAsync(
            organizationId,
            resourceAttributeId,
            request.Title,
            request.Description);

        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("/attributes/{resourceAttributeId}")]
    public async Task<IActionResult> DeleteResourceAttributeAsync(Guid resourceId, Guid resourceAttributeId)
    {
        logger.LogInformation("Delete resource attribute endpoint was called for resource attribute {ResourceAttributeId}", resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceAttributeService.DeleteResourceAttributeAsync(organizationId, resourceAttributeId);

        return NoContent();
    }
    
    [Authorize]
    [HttpPost("/attribute-assignments")]
    public async Task<IActionResult> CreateResourceAttributeAssignmentAsync([FromBody] CreateResourceAttributeAssignmentRequest request)
    {
        logger.LogInformation("Create resource attribute assignment endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var resourceAttributeId = await resourceAttributeAssignmentService.CreateResourceAttributeAssignmentAsync(
            request.ResourceId,
            request.ResourceAttributeId,
            request.OrganizationId);
        
        return CreatedAtAction(nameof(GetResourceAttributeAssignment), new { request.ResourceId, resourceAttributeId }, new { id = resourceAttributeId });
    }
    
    [Authorize]
    [HttpGet("/attribute-assignments/{resourceId}/{resourceAttributeId}")]
    public async Task<IActionResult> GetResourceAttributeAssignment(Guid resourceId, Guid resourceAttributeId)
    {
        logger.LogInformation("Get resource attribute assignment endpoint was called for resource {ResourceId} and attribute {ResourceAttributeId}", resourceId, resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceAttributeAssignment = await resourceAttributeAssignmentService.GetResourceAttributeAssignmentAsync(resourceId, resourceAttributeId, organizationId);
        if (resourceAttributeAssignment == null)
            return NotFound();

        var resourceAttributeAssignmentResponse = new ResourceAttributeAssignmentResponse(resourceId, resourceAttributeId);
        
        return Ok(resourceAttributeAssignmentResponse);
    }
    
    [Authorize]
    [HttpGet("/attribute-assignments")]
    public async Task<IActionResult> GetAllResourceAttributeAssignmentsAsync()
    {
        logger.LogInformation("Get resource attribute assignments endpoint was called.");
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        var resourceAttributeAssignments = await resourceAttributeAssignmentService.GetAllResourceAttributeAssignmentsAsync(organizationId);

        var resourceAttributeAssignmentResponses = resourceAttributeAssignments
            .Select(raa => new ResourceAttributeAssignmentResponse(raa.ResourceId, raa.ResourceAttributeId))
            .ToList();
        
        return Ok(resourceAttributeAssignmentResponses);
    }
    
    [Authorize]
    [HttpPatch("/attribute-assignments/{resourceId}/{resourceAttributeId}")]
    public async Task<IActionResult> UpdateResourceAttributeAssignmentAsync(Guid resourceId, Guid resourceAttributeId, [FromBody] UpdateResourceAttributeAssignmentRequest request)
    {
        logger.LogInformation("Update resource attribute assignment endpoint was called for resource {ResourceId} and attribute {ResourceAttributeId}", resourceId, resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceAttributeAssignmentService.UpdateResourceAttributeAssignmentAsync(
            resourceId,
            resourceAttributeId,
            organizationId);

        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("/attribute-assignments/{resourceId}/{resourceAttributeId}")]
    public async Task<IActionResult> DeleteResourceAttributeAssignmentAsync(Guid resourceId, Guid resourceAttributeId)
    {
        logger.LogInformation("Delete resource attribute assignment endpoint was called for resource {ResourceId} and attribute {ResourceAttributeId}", resourceId, resourceAttributeId);
        var organizationId = new Guid(GetOrganizationIdFromContext());
        
        await resourceAttributeAssignmentService.DeleteResourceAttributeAssignmentAsync(resourceId, resourceAttributeId, organizationId);

        return NoContent();
    }
    
    private string GetOrganizationIdFromContext()
    {
        logger.LogDebug("Extracting organization ID from HttpContext.");
        var organizationId = HttpContext.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return organizationId;
    }
}