using Microsoft.EntityFrameworkCore;

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
}