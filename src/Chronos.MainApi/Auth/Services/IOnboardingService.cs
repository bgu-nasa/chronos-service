using Chronos.Domain.Auth;

namespace Chronos.MainApi.Auth.Services;

public interface IOnboardingService
{
    Task<Guid> CreateOrganizationAsync(string organizationName, string plan);
    Task OnboardAdminUserAsync(Guid organizationId, User admin);
}