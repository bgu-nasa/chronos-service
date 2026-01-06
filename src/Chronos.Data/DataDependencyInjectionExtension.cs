using Chronos.Data.Repositories.Auth;
using Chronos.Data.Repositories.Management;
using Microsoft.Extensions.DependencyInjection;

namespace Chronos.Data;

public static class DataDependencyInjectionExtension
{
    public static void AddServiceRepositories(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<IUserRepository, UserRepository>();

        // Management
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    }
}