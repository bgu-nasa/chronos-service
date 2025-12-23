using Chronos.Data.Repositories.Management;
using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;
using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Schedule.Services;

public class ScheduleValidationService(
    IOrganizationRepository organizationRepository,
    ISchedulingPeriodRepository schedulingPeriodRepository,
    ISlotRepository slotRepository,
    ILogger<ScheduleValidationService> logger)
{
    public async Task ValidateOrganizationAsync(Guid organizationId)
    {
        var organization = await organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.Deleted)
        {
            logger.LogWarning("Organization not found or deleted. OrganizationId: {OrganizationId}", organizationId);
            throw new NotFoundException("Organization not found");
        }
    }
    public async Task<SchedulingPeriod> ValidateAndGetSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId)
    {
        var period = await schedulingPeriodRepository.GetByIdAsync(schedulingPeriodId);

        if (period == null || period.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "SchedulingPeriod not found or does not belong to organization. SchedulingPeriodId: {SchedulingPeriodId}, OrganizationId: {OrganizationId}",
                schedulingPeriodId, organizationId);

            throw new NotFoundException("Scheduling period not found");
        }

        return period;
    }

    public async Task<Slot> ValidateAndGetSlotAsync(Guid organizationId, Guid slotId)
    {
        var slot = await slotRepository.GetByIdAsync(slotId);
        if (slot == null || slot.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "Slot not found or does not belong to organization. SlotId: {SlotId}, OrganizationId: {OrganizationId}",
                slotId, organizationId);
            throw new NotFoundException("Slot not found");
        }
        return slot;
    }
}
