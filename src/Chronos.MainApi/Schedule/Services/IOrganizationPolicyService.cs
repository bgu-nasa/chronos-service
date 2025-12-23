using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface IOrganizationPolicyService
{

    Task<OrganizationPolicy> CreatePolicyAsync(Guid organizationId, Guid schedulingPeriodId, string key, string value);
    
    Task<List<OrganizationPolicy>> GetAllPoliciesAsync(Guid organizationId);
    
    Task<List<OrganizationPolicy>> GetPoliciesBySchedulingPeriodIdsAsync(Guid organizationId ,Guid schedulingPeriodId);
    
    Task<OrganizationPolicy> UpdatePolicyAsync(Guid organizationId,Guid policyId, string key, string value);

    Task DeletePolicyAsync(Guid organizationId, Guid policyId);
}