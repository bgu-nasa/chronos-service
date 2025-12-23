using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface ISlotService
{
    Task<Guid> CreateSlotAsync(Guid organizationId, Guid schedulingPeriodId, string weekday, TimeSpan fromTime, TimeSpan toTime);

    Task<Slot> GetSlotAsync(Guid organizationId, Guid slotId);

    Task<List<Slot>> GetAllSlotsAsync(Guid organizationId);
    
    Task<List<Slot>> GetSlotsBySchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId);

    Task UpdateSlotAsync(Guid organizationId, Guid slotId, string weekday, TimeSpan fromTime, TimeSpan toTime);

    Task DeleteSlotAsync(Guid organizationId, Guid slotId);
}
