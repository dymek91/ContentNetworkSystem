using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContentNetworkSystem.Models;

namespace ContentNetworkSystem.Data
{
    public class ContentNetworkSystemContext : DbContext
    {
        public ContentNetworkSystemContext(DbContextOptions<ContentNetworkSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Wordpress> Wordpresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().ToTable("Project");
            modelBuilder.Entity<Group>().ToTable("Group")
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Content>().ToTable("Content");
            modelBuilder.Entity<Wordpress>().ToTable("Wordpress");
        }
    }
}
