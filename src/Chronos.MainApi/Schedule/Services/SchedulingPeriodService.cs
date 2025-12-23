using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Schedule.Services;

public class SchedulingPeriodService(
    ISchedulingPeriodRepository schedulingPeriodRepository,
    ScheduleValidationService validationService,
    ILogger<SchedulingPeriodService> logger) : ISchedulingPeriodService
{
    public async Task<Guid> CreateSchedulingPeriodAsync(Guid organizationId, string name, DateTime fromDate, DateTime toDate)
    {
        logger.LogInformation(
            "Creating scheduling period. OrganizationId: {OrganizationId}, Name: {Name}, FromDate: {FromDate}, ToDate: {ToDate}",
            organizationId, name, fromDate, toDate);
        validateDateRange(fromDate, toDate);
        await validationService.ValidateOrganizationAsync(organizationId);

        var period = new SchedulingPeriod
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = name,
            FromDate = fromDate,
            ToDate = toDate
        };

        await schedulingPeriodRepository.AddAsync(period);

        logger.LogInformation(
            "Scheduling period created successfully. SchedulingPeriodId: {SchedulingPeriodId}, OrganizationId: {OrganizationId}",
            period.Id, organizationId);

        return period.Id;
    }

    public async Task<SchedulingPeriod> GetSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        logger.LogDebug(
            "Retrieving scheduling period. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            organizationId, schedulingPeriodId);

        return await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);
    }

    public async Task<List<SchedulingPeriod>> GetSchedulingPeriodsAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all scheduling periods for organization. OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var all = await schedulingPeriodRepository.GetAllAsync();
        var filtered = all
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.FromDate)
            .ToList();

        logger.LogDebug("Retrieved {Count} scheduling periods for organization. OrganizationId: {OrganizationId}", filtered.Count, organizationId);
        return filtered;
    }

    public async Task UpdateSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId, string name, DateTime fromDate, DateTime toDate)
    {
        logger.LogInformation(
            "Updating scheduling period. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            organizationId, schedulingPeriodId);

        validateDateRange(fromDate, toDate);

        var period = await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);

        period.Name = name;
        period.FromDate = fromDate;
        period.ToDate = toDate;

        await schedulingPeriodRepository.UpdateAsync(period);

        logger.LogInformation("Scheduling period updated successfully. SchedulingPeriodId: {SchedulingPeriodId}", schedulingPeriodId);
    }

    public async Task DeleteSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        logger.LogInformation(
            "Deleting scheduling period. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            organizationId, schedulingPeriodId);

        var period = await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);
        await schedulingPeriodRepository.DeleteAsync(period);

        logger.LogInformation("Scheduling period deleted successfully. SchedulingPeriodId: {SchedulingPeriodId}", schedulingPeriodId);
    }
    public void validateDateRange(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
        {
            throw new BadRequestException("FromDate must be before or equal to ToDate");
        }
    }

}
