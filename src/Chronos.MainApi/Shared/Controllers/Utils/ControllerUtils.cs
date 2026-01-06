using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;

namespace Chronos.MainApi.Shared.Controllers.Utils;

public static class ControllerUtils
{
    internal static Guid GetOrganizationIdAndFailIfMissing(HttpContext context, ILogger logger)
    {
        var organizationId = context.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        if (!Guid.TryParse(organizationId, out var organizationIdGuid))
        {
            logger.LogInformation("Received ill formatted organization ID in the header.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return organizationIdGuid;
    }
}