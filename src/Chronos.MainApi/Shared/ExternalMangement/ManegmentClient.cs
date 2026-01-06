using Chronos.MainApi.Management.Services;

namespace Chronos.MainApi.Shared.ExternalMangement;

public class ManagementExternalService(ManagementValidationService managementValidationService) : IManagementExternalService
{
    public async Task ValidateOrganizationAsync(Guid organizationId)
    {
        await managementValidationService.ValidateOrganizationAsync(organizationId);
    }
}