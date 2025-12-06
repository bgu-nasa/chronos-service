using System.Reflection;
using Chronos.Domain.Auth;
using Chronos.Domain.Course;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Context;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IHttpContextAccessor httpContextAccessor
) : DbContext(options)
{
    private readonly string? _currentOrganizationId = httpContextAccessor
        .HttpContext.GetOrganizationId()
        ?.ToLower();

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Add your entities here to make sure no leaks between orgs (tenants)
        if (_currentOrganizationId is not null)
        {
            modelBuilder
                .Entity<User>()
                .HasQueryFilter(u =>
                    u.OrganizationId.ToString().ToLower() == _currentOrganizationId
                );
        }
    }
}
