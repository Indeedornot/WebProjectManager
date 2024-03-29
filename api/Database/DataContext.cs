﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using shared.Models;

namespace api.Database;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("ProjectDatabase"));
    }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany(u => u.Assignees)
            .WithMany(p => p.Projects)
            .UsingEntity(j => j.ToTable("UserProject"));
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (EntityEntry? entity in entities)
        {
            DateTime now = DateTime.UtcNow; // current datetime
            var baseEntity = (BaseEntity)entity.Entity;

            if (entity.State == EntityState.Added)
            {
                baseEntity.CreatedAt = now;
            }

            baseEntity.UpdatedAt = now;
        }
    }
}
