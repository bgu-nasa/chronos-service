using Chronos.Data.Repositories.Management;
using Chronos.Domain.Management;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Management.Services;

public class OrganizationService(
    IOrganizationRepository organizationRepository,
    ILogger<OrganizationService> logger) : IOrganizationService
{
    public async Task<Guid> CreateOrganizationAsync(string name)
    {
        logger.LogInformation("Creating organization with name: {Name}", name);
        
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            Deleted = false
        };

        await organizationRepository.AddAsync(organization);
        
        logger.LogInformation("Organization created successfully with ID: {OrganizationId}", organization.Id);
        return organization.Id;
    }

    public async Task<Organization> GetOrganizationAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving organization with ID: {OrganizationId}", organizationId);
        
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization not found");
        }

        return organization;
    }

    public async Task UpdateOrganizationAsync(Guid organizationId, string name)
    {
        logger.LogInformation("Updating organization. OrganizationId: {OrganizationId}, NewName: {Name}", organizationId, name);
        
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization not found");
        }

        organization.Name = name;
        await organizationRepository.UpdateAsync(organization);
        
        logger.LogInformation("Organization updated successfully. OrganizationId: {OrganizationId}", organizationId);
    }

    public async Task SetForDeletionAsync(Guid organizationId)
    {
        logger.LogInformation("Setting organization for deletion. OrganizationId: {OrganizationId}", organizationId);
        
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null)
        {
            logger.LogWarning("Organization not found. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization not found");
        }

        if (organization.Deleted)
        {
            logger.LogWarning("Organization already set for deletion. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization is already set for deletion");
        }

        organization.Deleted = true;
        organization.DeletedTime = DateTime.UtcNow;
        await organizationRepository.UpdateAsync(organization);
        
        logger.LogInformation("Organization set for deletion successfully. OrganizationId: {OrganizationId}", organizationId);
    }

    public async Task RestoreDeletedOrganizationAsync(Guid organizationId)
    {
        logger.LogInformation("Restoring deleted organization. OrganizationId: {OrganizationId}", organizationId);
        
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null)
        {
            logger.LogWarning("Organization not found. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization not found");
        }

        if (!organization.Deleted)
        {
            logger.LogWarning("Organization is not set for deletion. OrganizationId: {OrganizationId}", organizationId);
            throw new BadRequestException("Organization is not set for deletion");
        }

        organization.Deleted = false;
        organization.DeletedTime = default;
        await organizationRepository.UpdateAsync(organization);
        
        logger.LogInformation("Organization restored successfully. OrganizationId: {OrganizationId}", organizationId);
    }
}