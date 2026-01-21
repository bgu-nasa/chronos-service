using Chronos.MainApi.Schedule.Services;

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

        // External Services
        services.AddScoped<IExternalActivityService, ExternalActivityService>();
        services.AddScoped<IExternalResourceService, ExternalResourceService>();
        services.AddScoped<IExternalSubjectService, ExternalSubjectService>();
    }
}