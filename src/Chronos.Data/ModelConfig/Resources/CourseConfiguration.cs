using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Course)));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DepartmentId)
            .IsRequired();

        builder.Property(c => c.SchedulingPeriodId)
            .IsRequired();

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(c => new { c.DepartmentId, c.Code })
            .IsUnique();
    }
}

