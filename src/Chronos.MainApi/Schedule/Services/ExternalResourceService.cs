using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;
using Chronos.MainApi.Shared.ExternalMangement;

namespace Chronos.MainApi.Schedule.Services;

public class ExternalResourceService(
    IExternalResourceRepository externalResourceRepository,
    IManagementExternalService validationService,
    ILogger<ExternalResourceService> logger) : IExternalResourceService
{
    public async Task<List<ExternalResource>> GetResourcesAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all external resources for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var allResources = await externalResourceRepository.GetAllAsync();
        var filteredResources = allResources
            .Where(r => r.OrganizationId == organizationId)
            .ToList();

        logger.LogDebug("Retrieved {Count} external resources for organization. OrganizationId: {OrganizationId}", filteredResources.Count, organizationId);
        return filteredResources;
    }

    public async Task<bool> ExternalResourceExistsAsync(Guid id , Guid organizationId)
    {
        await validationService.ValidateOrganizationAsync(organizationId);
        return await externalResourceRepository.ExistsAsync(id);
    }
      
      
}