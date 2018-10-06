using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Wycademy.Commands.Models
{
    public class MonsterContext : DbContext
    {
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<Hitzone> Hitzones { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<StaggerZone> Stagger { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.Combine(WycademyConst.DATA_LOCATION, "monster.db");

            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Monster>()
                .ToTable("monsters")
                .HasIndex(e => e.WebName)
                .HasName("sqlite_autoindex_monsters_1")
                .IsUnique();

            modelBuilder.Entity<Hitzone>()
                .ToTable("hitzones")
                .HasOne(h => h.Monster)
                .WithMany(m => m.Hitzones)
                .HasForeignKey(h => h.MonsterId);

            modelBuilder.Entity<Status>()
                .ToTable("status")
                .HasOne(s => s.Monster)
                .WithMany(m => m.Status)
                .HasForeignKey(s => s.MonsterId);

            modelBuilder.Entity<Item>()
                .ToTable("items")
                .HasOne(i => i.Monster)
                .WithMany(m => m.Items)
                .HasForeignKey(i => i.MonsterId);

            modelBuilder.Entity<StaggerZone>()
                .ToTable("stagger")
                .HasOne(s => s.Monster)
                .WithMany(m => m.Stagger)
                .HasForeignKey(s => s.MonsterId);
        }
    }
}
