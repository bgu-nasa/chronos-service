using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IOrganizationPolicyRepository
{
    Task<OrganizationPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<OrganizationPolicy>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OrganizationPolicy?> GetByPeriodAndKeyAsync(Guid schedulingPeriodId, string key, CancellationToken cancellationToken = default);

    Task AddAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default);

    Task UpdateAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default);

    Task DeleteAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

