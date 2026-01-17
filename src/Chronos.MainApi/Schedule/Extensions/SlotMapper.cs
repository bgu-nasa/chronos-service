using Chronos.Domain.Schedule;
using Chronos.MainApi.Schedule.Contracts;

namespace Chronos.MainApi.Schedule.Extensions;

public static class SlotMapper
{
    public static SlotResponse ToSlotResponse(this Slot slot) =>
        new(
            Id: slot.Id.ToString(),
            OrganizationId: slot.OrganizationId.ToString(),
            SchedulingPeriodId: slot.SchedulingPeriodId.ToString(),
            Weekday: Enum.Parse<WeekDays>(slot.Weekday, ignoreCase: true),
            FromTime: slot.FromTime,
            ToTime: slot.ToTime
        );
}
