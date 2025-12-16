using Chronos.MainApi.Management.Contracts;

namespace Chronos.MainApi.Management.Services;

public interface IOrganizationInfoService
{
    Task<OrganizationInformation> GetOrganizationInformationAsync(Guid organizationId, Guid userId);
}