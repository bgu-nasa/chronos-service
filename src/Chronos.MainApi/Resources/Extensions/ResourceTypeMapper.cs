using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Contracts;

namespace Chronos.MainApi.Resources.Extensions;

public static class ResourceTypeMapper
{
    public static ResourceTypeResponse ToResourceTypeResponse(this ResourceType resourceType) =>
        new(
            Id: resourceType.Id,
            OrganizationId: resourceType.OrganizationId,
            Type: resourceType.Type
        );
}

