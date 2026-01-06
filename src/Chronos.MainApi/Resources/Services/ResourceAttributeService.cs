using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Resources;

namespace Chronos.MainApi.Resources.Services;

public class ResourceAttributeService(
    IResourceAttributeRepository resourceAttributeRepository,
    ResourceValidationService validationService,
    ILogger<ResourceAttributeService> logger) : IResourceAttributeService
{
    public async Task<ResourceAttribute> CreateResourceAttributeAsync(Guid organizationId, string title, string description)
    {
        logger.LogInformation("Creating resource attribute. OrganizationId: {OrganizationId}, Title: {Title}, Description: {Description}",
            organizationId, title, description);

        await validationService.ValidationOrganizationAsync(organizationId);

        var resourceAttribute = new ResourceAttribute
        {
            OrganizationId = organizationId,
            Title = title,
            Description = description
        };

        await resourceAttributeRepository.AddAsync(resourceAttribute);

        logger.LogInformation("Resource attribute created successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttribute.Id, organizationId);
        return resourceAttribute;
    }

    public async Task<ResourceAttribute> GetResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId)
    {
        logger.LogDebug("Retrieving resource attribute. OrganizationId: {OrganizationId}, ResourceAttributeId: {ResourceAttributeId}", organizationId, resourceAttributeId);

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceAttribute = await validationService.ValidateAndGetResourceAttributeAsync(organizationId, resourceAttributeId);
        return resourceAttribute;
    }

    public async Task<List<ResourceAttribute>> GetResourceAttributesAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all resource attributes for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidationOrganizationAsync(organizationId);

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

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceAttribute = await validationService.ValidateAndGetResourceAttributeAsync(organizationId, resourceAttributeId);

        resourceAttribute.Title = title;
        resourceAttribute.Description = description;
        await resourceAttributeRepository.UpdateAsync(resourceAttribute);

        logger.LogInformation("Resource attribute updated successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttribute.Id, organizationId);
    }

    public async Task DeleteResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId)
    {
        logger.LogInformation("Deleting resource attribute. OrganizationId: {OrganizationId}, ResourceAttributeId: {ResourceAttributeId}", organizationId, resourceAttributeId);

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceAttribute = await validationService.ValidateAndGetResourceAttributeAsync(organizationId, resourceAttributeId);
        await resourceAttributeRepository.DeleteAsync(resourceAttribute);

        logger.LogInformation("Resource attribute deleted successfully. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttributeId, organizationId);
    }
}