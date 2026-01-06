namespace Chronos.MainApi.Resources.Contracts;

public sealed record UpdateResourceAttributeAssignmentRequest(
    Guid OrganizationId,
    Guid ResourceId,
    Guid ResourceAttributeId);