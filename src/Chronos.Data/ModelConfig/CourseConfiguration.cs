using Chronos.Data.Utils;
using Chronos.Domain.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Course)));

        // Key config:
        builder.HasKey(c => c.Id);

        // Property config:
        builder.Property(c => c.OrganizationId).IsRequired();
        builder.Property(c => c.DepartmentId).IsRequired();
        builder.Property(c => c.SchedulingPeriodId).IsRequired();
        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.EstimatedEnrollment).IsRequired();

        // ObjectInformation base class properties
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        // Indexes for performance
        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => c.DepartmentId);
        builder.HasIndex(c => c.SchedulingPeriodId);
        builder.HasIndex(c => c.Code);

        // Navigation properties
        builder
            .HasMany(c => c.Activities)
            .WithOne()
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
