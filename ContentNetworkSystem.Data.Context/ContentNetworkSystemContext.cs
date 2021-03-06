using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Models.GoogleSearchCache;

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
        public DbSet<Scheduler> Schedulers { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Wordpress> Wordpresses { get; set; }
        public DbSet<Niche> Niches { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<YoutubeResult> YoutubeResults { get; set; }
        public DbSet<ImagesResult> ImagesResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().ToTable("Project")
                .Property(c => c.DateAdded)
                .HasDefaultValueSql("now()");
            modelBuilder.Entity<Project>()
                .Property(c => c.WasSuccess)
                .HasDefaultValue(true);
            modelBuilder.Entity<Project>()
                  .HasOne(a => a.Content)
                  .WithOne(b => b.Project)
                  .HasForeignKey<Content>(b => b.ProjectId);
            modelBuilder.Entity<Group>().ToTable("Group")
                .HasIndex(e => e.Name)
                .IsUnique(); 
            modelBuilder.Entity<Scheduler>().ToTable("Scheduler");
            modelBuilder.Entity<Content>().ToTable("Content");
            modelBuilder.Entity<Niche>().ToTable("Niche")
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Keyword>().ToTable("Keyword")
                .HasIndex(e => new { e.NicheId, e.Name })
                .IsUnique();
            modelBuilder.Entity<YoutubeResult>().ToTable("YoutubeResult");
            modelBuilder.Entity<ImagesResult>().ToTable("ImagesResult");
        }
    }
}
