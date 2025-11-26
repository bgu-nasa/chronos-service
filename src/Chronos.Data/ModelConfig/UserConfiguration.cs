using Chronos.Data.Utils;
using Chronos.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronos.Data.ModelConfig;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(ConfigUtils.ToTableName(nameof(User)));

        // Key config:
        builder.HasKey(u => u.Id);

        // Property config:
        builder.Property(u => u.OrganizationId)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.AvatarUrl);
    }
}