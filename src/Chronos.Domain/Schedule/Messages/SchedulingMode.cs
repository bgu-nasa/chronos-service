namespace Chronos.Domain.Schedule.Messages;

public enum SchedulingMode
{
    Batch,   // Full period scheduling (Ranking algorithm)
    Online   // Incremental updates (Online matching)
}

