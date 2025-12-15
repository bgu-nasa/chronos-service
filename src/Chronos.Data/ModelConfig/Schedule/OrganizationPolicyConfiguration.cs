using Chronos.Data.Utils;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Schedule;

public class OrganizationPolicyConfiguration : IEntityTypeConfiguration<OrganizationPolicy>
{
    public void Configure(EntityTypeBuilder<OrganizationPolicy> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(OrganizationPolicy)));

        builder.HasKey(op => op.Id);

        builder.Property(op => op.OrganizationId)
            .IsRequired();

        builder.Property(op => op.SchedulingPeriodId)
            .IsRequired();

        builder.Property(op => op.Key)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(op => op.Value)
            .IsRequired()
            .HasMaxLength(1000);
    }
}

