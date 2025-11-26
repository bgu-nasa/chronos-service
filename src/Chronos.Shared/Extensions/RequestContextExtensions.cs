using Chronos.Shared.Constants;
using Microsoft.AspNetCore.Http;

namespace Chronos.Shared.Extensions;

public static class RequestContextExtensions
{
    public static string? GetOrganizationId(this HttpContext context) => context.Items.TryGetValue(RequestsConstants.OrganizationContextKey, out var orgId) ? orgId?.ToString() : null;
}