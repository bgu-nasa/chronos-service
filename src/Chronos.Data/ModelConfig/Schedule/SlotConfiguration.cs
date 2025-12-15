using Chronos.Data.Utils;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Schedule;

public class SlotConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Slot)));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SchedulingPeriodId)
            .IsRequired();

        builder.Property(s => s.Weekday)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.FromTime)
            .IsRequired();

        builder.Property(s => s.ToTime)
            .IsRequired();
    }
}

