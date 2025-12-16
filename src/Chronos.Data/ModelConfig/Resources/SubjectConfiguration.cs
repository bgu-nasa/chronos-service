using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Subject)));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.DepartmentId)
            .IsRequired();

        builder.Property(s => s.SchedulingPeriodId)
            .IsRequired();

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(s => new { s.DepartmentId, s.Code })
            .IsUnique();
    }
}

