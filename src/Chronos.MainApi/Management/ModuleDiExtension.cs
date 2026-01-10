using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Management.Services.External;

namespace Chronos.MainApi.Management;

public static class ModuleDiExtension
{
    public static void AddManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Validation Service
        services.AddScoped<ManagementValidationService>();

        // Services
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOrganizationInfoService, OrganizationInfoService>();
        services.AddScoped<IAuthClient, AuthClient>();
    }
}