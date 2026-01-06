using Chronos.Domain.Schedule;
using Chronos.MainApi.Management.Services;
namespace Chronos.MainApi.Schedule.Services;

public class ManegmentClient(ManagementValidationService managementValidationService) : IManegmentClient
{
    public async Task ValidateOrganizationAsync(Guid organizationId)
    {
        await managementValidationService.ValidateOrganizationAsync(organizationId);
    }
}