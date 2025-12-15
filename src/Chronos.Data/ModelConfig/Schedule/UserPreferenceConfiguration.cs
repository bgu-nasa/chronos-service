using Chronos.Data.Utils;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Schedule;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(UserPreference)));

        builder.HasKey(up => up.Id);

        builder.Property(up => up.UserId)
            .IsRequired();

        builder.Property(up => up.OrganizationId)
            .IsRequired();

        builder.Property(up => up.SchedulingPeriodId)
            .IsRequired();

        builder.Property(up => up.Key)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(up => up.Value)
            .IsRequired()
            .HasMaxLength(1000);
    }
}

