using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Contracts;

namespace Chronos.MainApi.Resources.Extensions;

public static class ResourceAttributeMapper
{
    public static ResourceAttributeResponse ToResourceAttributeResponse(this ResourceAttribute resourceAttribute) =>
        new(
            Id: resourceAttribute.Id,
            OrganizationId: resourceAttribute.OrganizationId,
            Title: resourceAttribute.Title,
            Description: resourceAttribute.Description
        );
}

