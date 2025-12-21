using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Resources;

namespace Chronos.MainApi.Resources.Services;

public class ResourceAttributeService(
    IResourceAttributeRepository resourceAttributeRepository,
    ILogger<ResourceAttributeService> logger) : IResourceAttributeService
{
    public async Task<Guid> CreateResourceAttributeAsync(Guid organizationId, string title, string description)
    {
        logger.LogInformation("Creating resource attribute. OrganizationId: {OrganizationId}, Title: {Title}, Description: {Description}",
            organizationId, title, description);
        var resourceAttribute = new ResourceAttribute
        {
            OrganizationId = organizationId,
            Title = title,
            Description = description
        };
        
        await resourceAttributeRepository.AddAsync(resourceAttribute);
        
        logger.LogInformation("Resource attribute created successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttribute.Id, organizationId);
        return resourceAttribute.Id;
    }

    public async Task<ResourceAttribute> GetResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId)
    {
        logger.LogDebug("Retrieving resource attribute. OrganizationId: {OrganizationId}, ResourceAttributeId: {ResourceAttributeId}", organizationId, resourceAttributeId);
        
        var resourceAttribute = await resourceAttributeRepository.GetByIdAsync(resourceAttributeId);
        // TODO: validate?
        return resourceAttribute;
    }

    public async Task<List<ResourceAttribute>> GetResourceAttributesAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all resource attributes for organization. OrganizationId: {OrganizationId}", organizationId);
        
        var allResourceAttributes = await resourceAttributeRepository.GetAllAsync();
        var filteredResourceAttributes = allResourceAttributes
            .Where(ra => ra.OrganizationId == organizationId)
            .ToList();
        
        logger.LogDebug("Retrieved {Count} resource attributes for organization. OrganizationId: {OrganizationId}", filteredResourceAttributes.Count, organizationId);
        return filteredResourceAttributes;
    }

    public async Task UpdateResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId, string title, string description)
    {
        logger.LogInformation("Updating resource attribute. OrganizationId: {OrganizationId}, ResourceAttributeId: {ResourceAttributeId}, Title: {Title}, Description: {Description}",
            organizationId, resourceAttributeId, title, description);
        
        var resourceAttribute = await resourceAttributeRepository.GetByIdAsync(resourceAttributeId);
        // TODO: validate?
        resourceAttribute.Title = title;
        resourceAttribute.Description = description;
        await resourceAttributeRepository.UpdateAsync(resourceAttribute);
        
        logger.LogInformation("Resource attribute updated successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttribute.Id, organizationId);
    }

    public async Task DeleteResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId)
    {
        logger.LogDebug("Deleting resource attribute. OrganizationId: {OrganizationId}, ResourceAttributeId: {ResourceAttributeId}", organizationId, resourceAttributeId);
        
        var resourceAttribute = await resourceAttributeRepository.GetByIdAsync(resourceAttributeId);
        // TODO: validate?
        await resourceAttributeRepository.DeleteAsync(resourceAttribute);
        
        logger.LogInformation("Resource attribute deleted successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttributeId, organizationId);
    }
}