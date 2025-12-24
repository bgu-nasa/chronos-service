namespace Chronos.MainApi.Resources.Contracts;

public sealed record UpdateResourceRequest(
    Guid ResourceTypeId,
    string Location,
    string Identifier,
    int? Capacity);