using Chronos.Data.Repositories.Management;
using Chronos.Data.Repositories.Resources;
using Chronos.Domain.Resources;
using Chronos.MainApi.Management.Services;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Resources.Services;

public class ResourceValidationService(
    IOrganizationService organizationService,
    IResourceRepository resourceRepository,
    IResourceTypeRepository resourceTypeRepository,
    IResourceAttributeRepository resourceAttributeRepository,
    IResourceAttributeAssignmentRepository resourceAttributeAssignmentRepository,
    ILogger<ResourceValidationService> logger)
{
    public async Task ValidationOrganizationAsync(Guid organizationId)
    {
        var organization = await organizationService.GetOrganizationAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new NotFoundException("Organization not found");
        }
    }

    public async Task<Resource> ValidateAndGetResourceAsync(Guid organizationId, Guid resourceId)
    {
        var resource = await resourceRepository.GetByIdAsync(resourceId);

        if (resource == null || resource.OrganizationId != organizationId)
        {
            logger.LogWarning("Resource not found or does not belong to organization. ResourceId: {ResourceId}, OrganizationId: {OrganizationId}", resourceId, organizationId);
            throw new NotFoundException("Resource not found");
        }

        return resource;
    }

    public async Task<ResourceType> ValidateAndGetResourceTypeAsync(Guid organizationId, Guid resourceTypeId)
    {
        var resourceType = await resourceTypeRepository.GetByIdAsync(resourceTypeId);

        if (resourceType == null || resourceType.OrganizationId != organizationId)
        {
            logger.LogWarning("Resource type not found or does not belong to organization. ResourceTypeId: {ResourceTypeId}, OrganizationId: {OrganizationId}", resourceTypeId, organizationId);
            throw new NotFoundException("Resource type not found");
        }

        return resourceType;
    }

    public async Task<ResourceAttribute> ValidateAndGetResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId)
    {
        var resourceAttribute = await resourceAttributeRepository.GetByIdAsync(resourceAttributeId);

        if (resourceAttribute == null || resourceAttribute.OrganizationId != organizationId)
        {
            logger.LogWarning("Resource attribute not found or does not belong to organization. ResourceAttributeId: {ResourceAttributeId}, OrganizationId: {OrganizationId}", resourceAttributeId, organizationId);
            throw new NotFoundException("Resource attribute not found");
        }

        return resourceAttribute;
    }

    public async Task<ResourceAttributeAssignment> ValidateAndGetResourceAttributeAssignmentAsync(Guid organizationId, Guid resourceId, Guid resourceAttributeId)
    {
        var resourceAttributeAssignment = await resourceAttributeAssignmentRepository.GetByIdAsync(resourceId, resourceAttributeId);

        if (resourceAttributeAssignment == null)
        {
            logger.LogWarning("Resource attribute assignment not found. ResourceId: {ResourceId}, ResourceAttributeId: {ResourceAttributeId}", resourceId, resourceAttributeId);
            throw new NotFoundException("Resource attribute assignment not found");
        }

        return resourceAttributeAssignment;
    }
}