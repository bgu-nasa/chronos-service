using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Resources;

namespace Chronos.MainApi.Resources.Services;

public class ResourceTypeService(
    IResourceTypeRepository resourceTypeRepository,
    ResourceValidationService validationService,
    ILogger<ResourceType> logger) : IResourceTypeService
{
    public async Task<ResourceType> CreateResourceTypeAsync(Guid organizationId, string type)
    {
        logger.LogInformation("Creating resource type. OrganizationId: {OrganizationId}, Type: {Type}",
            organizationId, type);

        await validationService.ValidationOrganizationAsync(organizationId);

        var resourceType = new ResourceType
        {
            OrganizationId = organizationId,
            Type = type
        };

        await resourceTypeRepository.AddAsync(resourceType);

        logger.LogInformation("Resource type created successfully. ResourceTypeId: {ResourceTypeId}, OrganizationId: {OrganizationId}", resourceType.Id, organizationId);
        return resourceType;
    }

    public async Task<ResourceType> GetResourceTypeAsync(Guid organizationId, Guid resourceTypeId)
    {
        logger.LogDebug("Retrieving resource type. OrganizationId: {OrganizationId}, ResourceTypeId: {ResourceTypeId}", organizationId, resourceTypeId);

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceType = await validationService.ValidateAndGetResourceTypeAsync(organizationId, resourceTypeId);
        return resourceType;
    }

    public async Task<List<ResourceType>> GetResourceTypesAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all resource types for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidationOrganizationAsync(organizationId);

        var allResourceTypes = await resourceTypeRepository.GetAllAsync();
        var filteredResourceTypes = allResourceTypes
            .Where(rt => rt.OrganizationId == organizationId)
            .ToList();

        logger.LogDebug("Retrieved {Count} resource types for organization. OrganizationId: {OrganizationId}", filteredResourceTypes.Count, organizationId);
        return filteredResourceTypes;
    }

    public async Task UpdateResourceTypeAsync(Guid organizationId, Guid resourceTypeId, string type)
    {
        logger.LogInformation("Updating resource type. OrganizationId: {OrganizationId}, ResourceTypeId: {ResourceTypeId}, Type: {Type}",
            organizationId, resourceTypeId, type);

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceType = await validationService.ValidateAndGetResourceTypeAsync(organizationId, resourceTypeId);

        resourceType.Type = type;
        await resourceTypeRepository.UpdateAsync(resourceType);

        logger.LogInformation("Resource type updated successfully. ResourceTypeId: {ResourceTypeId}, OrganizationId: {OrganizationId}", resourceType.Id, organizationId);
    }

    public async Task DeleteResourceTypeAsync(Guid organizationId, Guid resourceTypeId)
    {
        logger.LogInformation("Deleting resource type. OrganizationId: {OrganizationId}, ResourceTypeId: {ResourceTypeId}", organizationId, resourceTypeId);

        await validationService.ValidationOrganizationAsync(organizationId);
        var resourceType = await validationService.ValidateAndGetResourceTypeAsync(organizationId, resourceTypeId);
        await resourceTypeRepository.DeleteAsync(resourceType);

        logger.LogInformation("Resource type deleted successfully. ResourceTypeId: {ResourceTypeId}, OrganizationId: {OrganizationId}", resourceTypeId, organizationId);
    }
}