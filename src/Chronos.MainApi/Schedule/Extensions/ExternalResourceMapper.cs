using Chronos.Domain.Schedule;
using Chronos.MainApi.Schedule.Contracts;

namespace Chronos.MainApi.Schedule.Extensions;

public static class ExternalResourceMapper
{
    public static ExternalResourceResponse ToResourceResponse(this ExternalResource resource) =>
        new(
            Id: resource.Id,
            OrganizationId: resource.OrganizationId,
            ResourceTypeId: resource.ResourceTypeId,
            Location: resource.Location,
            Identifier: resource.Identifier,
            Capacity: resource.Capacity
        );
}

