namespace Chronos.MainApi.Shared.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment environment)
    {
        return environment.IsEnvironment("Local");
    }
}