namespace Chronos.MainApi.Resources.Contracts;

public sealed record UpdateResourceAttributeRequest(
    string Title,
    string? Description);