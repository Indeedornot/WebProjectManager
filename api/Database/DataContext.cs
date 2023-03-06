using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using shared.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Database;
public class DataContext : DbContext {
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration) {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("ProjectDatabase"));
    }

    public DbSet<Project> Projects { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<User>()
            .HasMany(u => u.Projects)
            .WithMany(p => p.Assignees)
            .UsingEntity(j => j.ToTable("UserProject"));
    }

    public override int SaveChanges() {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps() {
        IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities) {
            var now = DateTime.UtcNow; // current datetime
            var baseEntity = (BaseEntity)entity.Entity;

            if (entity.State == EntityState.Added) {
                baseEntity.CreatedAt = now;
            }
            baseEntity.UpdatedAt = now;
        }
    }
}