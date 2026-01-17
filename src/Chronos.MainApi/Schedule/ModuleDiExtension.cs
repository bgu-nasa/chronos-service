using Chronos.MainApi.Schedule.Services;
using Chronos.MainApi.Shared.ExternalMangement;

namespace Chronos.MainApi.Schedule;

public static class ModuleDiExtension
{
    public static void AddScheduleModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<ISchedulingPeriodService, SchedulingPeriodService>();
        services.AddScoped<ISlotService, SlotService>();
        services.AddScoped<IAssignmentService, AssignmentService>();

        services.AddScoped<IActivityConstraintService, ActivityConstraintService>();
        services.AddScoped<IOrganizationPolicyService, OrganizationPolicyService>();
        services.AddScoped<IUserPreferenceService, UserPreferenceService>();
        services.AddScoped<IUserConstraintService, UserConstraintService>();

        services.AddScoped<IManagementExternalService, ManagementExternalService>();


    }
}