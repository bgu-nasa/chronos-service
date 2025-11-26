
using Chronos.Shared.Constants;

namespace Chronos.MainApi.Shared.Middleware;

/// <summary>
/// Collects the organization id from the header if available.
/// </summary>
public class OrganizationMiddleware(RequestDelegate next)
{
    private const string OrganizationHeaderKey = "x-org-id";
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(OrganizationHeaderKey, out var organizationId))
        {
            context.Items[RequestsConstants.OrganizationContextKey] = organizationId.ToString();
        }
        
        await next(context);
    }
}