using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chronos.MainApi.Shared.Middleware;

/// <summary>
/// Decorator for controller classes and methods that require an organization id.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireOrganizationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.GetOrganizationId() is null)
        {
            context.Result = new BadRequestObjectResult("Request is missing organization id header.");
            return;
        }
        
        await next();
    }
}