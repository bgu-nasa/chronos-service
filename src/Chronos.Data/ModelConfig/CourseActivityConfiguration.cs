using Chronos.Data.Utils;
using Chronos.Domain.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class CourseActivityConfiguration : IEntityTypeConfiguration<CourseActivity>
{
    public void Configure(EntityTypeBuilder<CourseActivity> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(CourseActivity)));

        // Key config:
        builder.HasKey(ca => ca.Id);

        // Property config:
        builder.Property(ca => ca.CourseId).IsRequired();
        builder.Property(ca => ca.InstructorId).IsRequired();
        builder.Property(ca => ca.Type).IsRequired();
        builder.Property(ca => ca.DurationMinutes).IsRequired();

        // ObjectInformation base class properties
        builder.Property(ca => ca.CreatedAt).IsRequired();
        builder.Property(ca => ca.UpdatedAt).IsRequired();

        // Indexes for performance
        builder.HasIndex(ca => ca.CourseId);
        builder.HasIndex(ca => ca.InstructorId);
    }
}
