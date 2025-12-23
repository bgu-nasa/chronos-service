using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public class SlotService(
    ISlotRepository slotRepository,
    ScheduleValidationService validationService,
    ILogger<SlotService> logger) : ISlotService
{
    public async Task<Guid> CreateSlotAsync(
        Guid organizationId,
        Guid schedulingPeriodId,
        string weekday,
        TimeSpan fromTime,
        TimeSpan toTime)
    {
        logger.LogInformation(
            "Creating slot. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}, Weekday: {Weekday}, FromTime: {FromTime}, ToTime: {ToTime}",
            organizationId, schedulingPeriodId, weekday, fromTime, toTime);

        await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);
        ValidateInput(weekday, fromTime, toTime);

        var slot = new Slot
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            SchedulingPeriodId = schedulingPeriodId,
            Weekday = weekday.Trim(),
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

    public async Task<List<Slot>> GetSlotsBySchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        logger.LogDebug(
            "Retrieving slots by scheduling period. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            organizationId, schedulingPeriodId);

        await validationService.ValidateAndGetSchedulingPeriodAsync(organizationId, schedulingPeriodId);

        var slots = await slotRepository.GetBySchedulingPeriodIdAsync(schedulingPeriodId);

        // Defensive filter (should already match because period validated)
        var filtered = slots
            .Where(s => s.OrganizationId == organizationId)
            .OrderBy(s => s.Weekday)
            .ThenBy(s => s.FromTime)
            .ToList();

        logger.LogDebug(
            "Retrieved {Count} slots. OrganizationId: {OrganizationId}, SchedulingPeriodId: {SchedulingPeriodId}",
            filtered.Count, organizationId, schedulingPeriodId);

        return filtered;
    }

    public async Task UpdateSlotAsync(
        Guid organizationId,
        Guid slotId,
        string weekday,
        TimeSpan fromTime,
        TimeSpan toTime)
    {
        logger.LogInformation(
            "Updating slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);

        ValidateInput(weekday, fromTime, toTime);

        var slot = await validationService.ValidateAndGetSlotAsync(organizationId, slotId);

        slot.Weekday = weekday.Trim();
        slot.FromTime = fromTime;
        slot.ToTime = toTime;

        await slotRepository.UpdateAsync(slot);

        logger.LogInformation("Slot updated successfully. SlotId: {SlotId}", slotId);
    }

    public async Task DeleteSlotAsync(Guid organizationId, Guid slotId)
    {
        logger.LogInformation(
            "Deleting slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);

        var slot = await validationService.ValidateAndGetSlotAsync(organizationId, slotId);
        await slotRepository.DeleteAsync(slot);

        logger.LogInformation("Slot deleted successfully. SlotId: {SlotId}", slotId);
    }

    private static void ValidateInput(string weekday, TimeSpan fromTime, TimeSpan toTime)
    {
        if (string.IsNullOrWhiteSpace(weekday))
            throw new ArgumentException("Weekday is required", nameof(weekday));

        if (toTime <= fromTime)
            throw new ArgumentException("ToTime must be after FromTime");

        // Optional sanity (TimeSpan can exceed 24h)
        if (fromTime < TimeSpan.Zero || toTime < TimeSpan.Zero)
            throw new ArgumentException("Times must be non-negative");

        if (fromTime >= TimeSpan.FromDays(1) || toTime > TimeSpan.FromDays(1))
            throw new ArgumentException("Times must be within a single day (00:00 - 24:00)");
    }
}
