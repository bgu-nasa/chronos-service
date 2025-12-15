using Chronos.Data.Utils;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig.Resources;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(Activity)));

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CourseInstanceId)
            .IsRequired();

        builder.Property(a => a.LecturerUserId)
            .IsRequired();

        builder.Property(a => a.ActivityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ExpectedStudents);
    }
}

