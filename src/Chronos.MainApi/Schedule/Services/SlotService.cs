using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Schedule.Services;

public class SlotService(
    ISlotRepository slotRepository,
    ScheduleValidationService validationService,
    ILogger<SlotService> logger) : ISlotService
{
    

    public async Task<Guid> CreateSlotAsync(Guid organizationId, Guid schedulingPeriodId, string weekday, TimeSpan fromTime, TimeSpan toTime)
    {
        logger.LogInformation(
            "Creating slot. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}, Weekday: {Weekday}, FromTime: {FromTime}, ToTime: {ToTime}",
            organizationId, schedulingPeriodId, weekday, fromTime, toTime);
        await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);
        TimeRangeValidator(fromTime, toTime);

        var slot = new Slot
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            SchedulingPeriodId = schedulingPeriodId,
            Weekday = weekday,
            FromTime = fromTime,
            ToTime = toTime
        };

        await slotRepository.AddAsync(slot);

        logger.LogInformation(
            "Slot created successfully. SlotId: {SlotId}, OrganizationId: {OrganizationId}",
            slot.Id, organizationId);

        return slot.Id;
    }
    public async Task<Slot> GetSlotAsync(Guid organizationId, Guid slotId)
    {
        logger.LogDebug(
            "Retrieving slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);

        return await validationService.ValidateAndGetSlotAsync(organizationId, slotId);
    }
    
    public async Task<List<Slot>> GetAllSlotsAsync(Guid organizationId)
    {
        logger.LogDebug("Retrieving all slots OrganizationId: {OrganizationId}", organizationId);

        await validationService.ValidateOrganizationAsync(organizationId);

        var all = await slotRepository.GetAllAsync();
        var filtered = all
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.Weekday)
            .ThenBy(s => s.FromTime)
            .ToList();

        logger.LogDebug("Retrieved {Count} slots for scheduling period. OrganizationId: {OrganizationId}", filtered.Count, organizationId);
        return filtered;
    }
    
    public async Task<List<Slot>> GetSlotsBySchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        logger.LogDebug(
            "Retrieving slots by scheduling period. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            organizationId, schedulingPeriodId);

        await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);

        var all = await slotRepository.GetBySchedulingPeriodIdAsync(schedulingPeriodId);
        var filtered = all
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.Weekday)
            .ThenBy(s => s.FromTime)
            .ToList();

        logger.LogDebug("Retrieved {Count} slots for scheduling period. SchedulingPeriodId: {SchedulingPeriodId}, OrganizationId: {OrganizationId}", filtered.Count, schedulingPeriodId, organizationId);
        return filtered;
    }
    
    public async Task UpdateSlotAsync(Guid organizationId, Guid slotId, string weekday, TimeSpan fromTime, TimeSpan toTime)
    {
        logger.LogInformation(
            "Updating slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);

        var slot = await validationService.ValidateAndGetSlotAsync(organizationId, slotId);
        TimeRangeValidator(fromTime, toTime);
        slot.Weekday = weekday;
        slot.FromTime = fromTime;
        slot.ToTime = toTime;

        await slotRepository.UpdateAsync(slot);

        logger.LogInformation(
            "Slot updated successfully. SlotId: {SlotId}, OrganizationId: {OrganizationId}",
            slot.Id, organizationId);
    }
    
    public async Task DeleteSlotAsync(Guid organizationId, Guid slotId)
    {
        logger.LogInformation(
            "Deleting slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);

        var slot = await validationService.ValidateAndGetSlotAsync(organizationId, slotId);

        await slotRepository.DeleteAsync(slot);

        logger.LogInformation(
            "Slot deleted successfully. SlotId: {SlotId}, OrganizationId: {OrganizationId}",
            slot.Id, organizationId);
    }
    
    private void TimeRangeValidator(TimeSpan fromTime, TimeSpan toTime)
    {
        if (fromTime >= toTime)
        {
            logger.LogWarning(
                "Invalid time range. FromTime: {FromTime}, ToTime: {ToTime}",
                fromTime, toTime);
            throw new BadRequestException("FromTime must be earlier than ToTime");
        }
    }
}