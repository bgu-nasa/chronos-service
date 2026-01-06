using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Contracts;

namespace Chronos.MainApi.Resources.Extensions;

public static class ResourceMapper
{
    public static ResourceResponse ToResourceResponse(this Resource resource) =>
        new(
            Id: resource.Id,
            OrganizationId: resource.OrganizationId,
            ResourceTypeId: resource.ResourceTypeId,
            Location: resource.Location,
            Identifier: resource.Identifier,
            Capacity: resource.Capacity
        );
}

