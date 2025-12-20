using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;

namespace Chronos.MainApi.Management.Controllers;

public static class ControllerUtils
{
    internal static string GetOrganizationIdAndFailIfMissing(HttpContext context, ILogger logger)
    {
        var organizationId = context.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return organizationId;
    }
}