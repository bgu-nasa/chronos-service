using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Contracts;

namespace Chronos.MainApi.Resources.Extensions;

public static class ResourceAttributeAssignmentMapper
{
    public static ResourceAttributeAssignmentResponse ToResourceAttributeAssignmentResponse(
        this ResourceAttributeAssignment assignment) =>
        new(
            ResourceId: assignment.ResourceId,
            ResourceAttributeId: assignment.ResourceAttributeId,
            OrganizationId: assignment.OrganizationId
        );
}

