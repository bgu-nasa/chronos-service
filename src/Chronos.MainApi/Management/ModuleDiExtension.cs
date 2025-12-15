using Chronos.MainApi.Management.Services;

namespace Chronos.MainApi.Management;

public static class ModuleDiExtension
{
    public static void AddManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
    }
}