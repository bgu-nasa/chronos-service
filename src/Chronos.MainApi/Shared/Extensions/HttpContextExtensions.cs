using Chronos.Shared.Constants;

namespace Chronos.MainApi.Shared.Extensions;

// TODO ; If you add the dependencies from here to Chronos.Shared, you can move this logic to
// TODO ; Chronos.Shared.Extensions/RequestContextExtensions.cs and delete this.
public static class HttpContextExtensions
{
    public static string? GetDepartmentId(this HttpContext context) => context.Request.RouteValues.TryGetValue(RequestsConstants.DepartmentRouteValueKey, out var deptId) ? deptId?.ToString() : null;
}