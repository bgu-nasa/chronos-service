using Chronos.Data.Repositories.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Chronos.Data;

public static class DataDependencyInjectionExtension
{
    public static void AddServiceRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}