namespace Chronos.MainApi.Shared.ExternalMangement;

public interface IManagementExternalService
{
    Task ValidateOrganizationAsync(Guid organizationId);
}