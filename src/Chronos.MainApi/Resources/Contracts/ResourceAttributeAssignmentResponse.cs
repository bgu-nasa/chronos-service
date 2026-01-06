namespace Chronos.MainApi.Resources.Contracts;

public record ResourceAttributeAssignmentResponse(
    Guid ResourceId,
    Guid ResourceAttributeId,
    Guid OrganizationId);