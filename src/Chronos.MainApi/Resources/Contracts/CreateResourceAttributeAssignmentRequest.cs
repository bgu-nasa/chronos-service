namespace Chronos.MainApi.Resources.Contracts;

public sealed record CreateResourceAttributeAssignmentRequest(
    Guid OrganizationId,
    Guid ResourceId,
    Guid ResourceAttributeId);