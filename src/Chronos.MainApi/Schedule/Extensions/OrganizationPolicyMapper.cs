using Chronos.Domain.Schedule;
using Chronos.MainApi.Schedule.Contracts;

namespace Chronos.MainApi.Schedule.Extensions;

public static class OrganizationPolicyMapper
{
    public static OrganizationPolicyResponse ToOrganizationPolicyResponse(this OrganizationPolicy policy) =>
        new(
            Id: policy.Id.ToString(),
            OrganizationId: policy.OrganizationId.ToString(),
            SchedulingPeriodId: policy.SchedulingPeriodId.ToString(),
            Key: policy.Key,
            Value: policy.Value
        );
}
