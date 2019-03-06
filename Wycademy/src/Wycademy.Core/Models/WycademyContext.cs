﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public class WycademyContext : DbContext
    {
        public WycademyContext()
        {
        }

        public WycademyContext(DbContextOptions<WycademyContext> options)
            : base(options)
        {
        }

        public DbSet<BowStats> BowStats { get; set; }
        public DbSet<GunCrouchingFireShot> GunCrouchingFireShots { get; set; }
        public DbSet<GunInternalShot> GunInternalShots { get; set; }
        public DbSet<GunRapidFireShot> GunRapidFireShots { get; set; }
        public DbSet<GunShot> GunShots { get; set; }
        public DbSet<GunStats> GunStats { get; set; }
        public DbSet<GunlanceShellStats> GunlanceShells { get; set; }
        public DbSet<Hitzone> Hitzones { get; set; }
        public DbSet<HornNotes> HornNotes { get; set; }
        public DbSet<ItemEffect> ItemEffects { get; set; }
        public DbSet<Monsters> Monsters { get; set; }
        public DbSet<Phial> Phials { get; set; }
        public DbSet<StaggerLimit> StaggerLimits { get; set; }
        public DbSet<StatusEffect> StatusEffects { get; set; }
        public DbSet<WeaponEffect> WeaponEffects { get; set; }
        public DbSet<WeaponLevel> WeaponLevels { get; set; }
        public DbSet<WeaponSharpness> WeaponSharpnesses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // TODO: Make sure all lazy loading is removed
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlHasEnum<Game>()
                .ForNpgsqlHasEnum<HornNote>()
                .ForNpgsqlHasEnum<WeaponEffectType>()
                .ForNpgsqlHasEnum<WeaponType>()
                .HasPostgresExtension("fuzzystrmatch");

            modelBuilder.ForNpgsqlUseIdentityByDefaultColumns();

            modelBuilder.Entity<BowStats>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ArcShot)
                    .IsRequired();

                entity.Property(e => e.ChargeShots)
                    .IsRequired();

                entity.Property(e => e.Coatings)
                    .IsRequired();

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.BowStats)
                    .HasForeignKey<BowStats>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunCrouchingFireShot>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.GunStatsId, e.Name });

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunCrouchingFireShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunInternalShot>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.GunStatsId, e.Name });

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunInternalShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunRapidFireShot>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.GunStatsId, e.Name });

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunRapidFireShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunShot>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.GunStats, e.Name });

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunStats>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Deviation)
                    .IsRequired();

                entity.Property(e => e.Recoil)
                    .IsRequired();

                entity.Property(e => e.ReloadSpeed)
                    .IsRequired();

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.GunStats)
                    .HasForeignKey<GunStats>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GunlanceShellStats>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ShellType)
                    .IsRequired();

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.GunlanceShellStats)
                    .HasForeignKey<GunlanceShellStats>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Hitzone>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.MonsterId, e.Game, e.Name });

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.Hitzones)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HornNotes>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Note1)
                    .IsRequired();

                entity.Property(e => e.Note2)
                    .IsRequired();

                entity.Property(e => e.Note3)
                    .IsRequired();

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.HornNotes)
                    .HasForeignKey<HornNotes>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemEffect>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.MonsterId, e.Game, e.Name });

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.ItemEffects)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Monsters>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ProperName)
                    .IsRequired();

                entity.Property(e => e.WebName)
                    .IsRequired();

                entity.HasAlternateKey(e => e.WebName);
            });

            modelBuilder.Entity<Phial>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PhialType)
                    .IsRequired();

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.Phials)
                    .HasForeignKey<Phial>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StaggerLimit>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.ExtractColour)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.MonsterId, e.Game, e.Name });

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.StaggerLimits)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasDiscriminator(e => e.Game)
                    .HasValue<StaggerLimit4U>(Game.Four)
                    .HasValue<StaggerLimitGen>(Game.Generations)
                    .HasValue<StaggerLimitWorld>(Game.World);
            });

            modelBuilder.Entity<StatusEffect>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.MonsterId, e.Game, e.Name });

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.StatusEffects)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WeaponEffect>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EffectType)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.WeaponLevelId, e.EffectType });

                entity.HasOne(d => d.WeaponLevel)
                    .WithMany(p => p.WeaponEffects)
                    .HasForeignKey(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WeaponLevel>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.Slots)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.WeaponId, e.LevelOrdinal });

                entity.HasOne(d => d.Weapon)
                    .WithMany(p => p.WeaponLevels)
                    .HasForeignKey(d => d.WeaponId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasDiscriminator(e => e.Game)
                    .HasValue<WeaponLevel4U>(Game.Four)
                    .HasValue<WeaponLevelGen>(Game.Generations)
                    .HasValue<WeaponLevelWorld>(Game.World);
            });

            modelBuilder.Entity<WeaponSharpness>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.HasAlternateKey(e => new { e.WeaponLevelId, e.SharpnessOrdinal });

                entity.HasOne(d => d.WeaponLevel)
                    .WithMany(p => p.WeaponSharpnesses)
                    .HasForeignKey(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Weapon>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Game)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.WeaponType)
                    .IsRequired();

                entity.Property(e => e.Rare)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .IsRequired();

                entity.HasAlternateKey(e => new { e.Game, e.Name, e.WeaponType });

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasDiscriminator(e => e.Game)
                    .HasValue<Weapon4U>(Game.Four)
                    .HasValue<WeaponGen>(Game.Generations)
                    .HasValue<WeaponWorld>(Game.World);
            });

            modelBuilder.Entity<WeaponGen>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired(); ;

                entity.Property(e => e.FinalDescription)
                    .IsRequired(); ;

                entity.Property(e => e.FinalName)
                    .IsRequired();
            });
        }
    }
}