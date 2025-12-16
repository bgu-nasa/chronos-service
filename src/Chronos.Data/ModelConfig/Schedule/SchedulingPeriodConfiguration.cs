using Chronos.Data.Utils;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Schedule;

public class SchedulingPeriodConfiguration : IEntityTypeConfiguration<SchedulingPeriod>
{
    public void Configure(EntityTypeBuilder<SchedulingPeriod> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(SchedulingPeriod)));

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.OrganizationId)
            .IsRequired();

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(sp => sp.FromDate)
            .IsRequired();

        builder.Property(sp => sp.ToDate)
            .IsRequired();

        builder.HasIndex(sp => new { sp.OrganizationId, sp.Name })
            .IsUnique();
    }
}

