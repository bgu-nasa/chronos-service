﻿using System.Reflection;
using Chronos.Domain.Auth;
using Chronos.Domain.Management;
using Chronos.Domain.Management.Roles;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Context;
public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
    : DbContext(options)
{
    private readonly string? _currentOrganizationId = httpContextAccessor.HttpContext.GetOrganizationId()?.ToLower();
    // Auth
    public DbSet<User> Users => Set<User>();
    // Management
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();

    // Resources
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceAttribute> ResourceAttributes => Set<ResourceAttribute>();
    public DbSet<ResourceType> ResourceTypes => Set<ResourceType>();
    public DbSet<ResourceAttributeAssignment> ResourceAttributeAssignments => Set<ResourceAttributeAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // Add your entities here to make sure no leaks between orgs (tenants)
        if (_currentOrganizationId is not null)
        {
            // Auth
            modelBuilder.Entity<User>().HasQueryFilter(u => u.OrganizationId.ToString().ToLower() == _currentOrganizationId);
            // Management
            modelBuilder.Entity<Organization>().HasQueryFilter(o => o.Id.ToString().ToLower() == _currentOrganizationId);
            modelBuilder.Entity<Department>().HasQueryFilter(d => d.OrganizationId.ToString().ToLower() == _currentOrganizationId);
            modelBuilder.Entity<RoleAssignment>().HasQueryFilter(ra => ra.OrganizationId.ToString().ToLower() == _currentOrganizationId);

            // Resources
            modelBuilder.Entity<Resource>().HasQueryFilter(r => r.OrganizationId.ToString().ToLower() == _currentOrganizationId);
            modelBuilder.Entity<ResourceAttribute>().HasQueryFilter(ra => ra.OrganizationId.ToString().ToLower() == _currentOrganizationId);
            modelBuilder.Entity<ResourceType>().HasQueryFilter(rt => rt.OrganizationId.ToString().ToLower() == _currentOrganizationId);
        }

    }
}