using Chronos.MainApi.Shared.ExternalMangement;

namespace Chronos.MainApi.Shared;

public static class ModuleDiExtension
{
    public static void AddSharedModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IManagementExternalService, ManagementExternalService>();
    }
}