using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Wycademy.Core.Models
{
    public partial class WycademyContext : DbContext
    {
        public WycademyContext()
        {
        }

        public WycademyContext(DbContextOptions<WycademyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BowStats> BowStats { get; set; }
        public virtual DbSet<GunCrouchingFireShots> GunCrouchingFireShots { get; set; }
        public virtual DbSet<GunInternalShots> GunInternalShots { get; set; }
        public virtual DbSet<GunRapidFireShots> GunRapidFireShots { get; set; }
        public virtual DbSet<GunShots> GunShots { get; set; }
        public virtual DbSet<GunStats> GunStats { get; set; }
        public virtual DbSet<GunlanceShells> GunlanceShells { get; set; }
        public virtual DbSet<Hitzones> Hitzones { get; set; }
        public virtual DbSet<HornNotes> HornNotes { get; set; }
        public virtual DbSet<ItemEffects> ItemEffects { get; set; }
        public virtual DbSet<Monsters> Monsters { get; set; }
        public virtual DbSet<Phials> Phials { get; set; }
        public virtual DbSet<StaggerLimitsCommon> StaggerLimitsCommon { get; set; }
        public virtual DbSet<StaggerLimitsGen> StaggerLimitsGen { get; set; }
        public virtual DbSet<StaggerLimitsWorld> StaggerLimitsWorld { get; set; }
        public virtual DbSet<StatusEffects> StatusEffects { get; set; }
        public virtual DbSet<WeaponEffects> WeaponEffects { get; set; }
        public virtual DbSet<WeaponLevels4u> WeaponLevels4u { get; set; }
        public virtual DbSet<WeaponLevelsCommon> WeaponLevelsCommon { get; set; }
        public virtual DbSet<WeaponLevelsWorld> WeaponLevelsWorld { get; set; }
        public virtual DbSet<WeaponSharpnesses> WeaponSharpnesses { get; set; }
        public virtual DbSet<WeaponsCommon> WeaponsCommon { get; set; }
        public virtual DbSet<WeaponsGen> WeaponsGen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlHasEnum(null, "game_enum", new[] { "four", "generations", "world" })
                .ForNpgsqlHasEnum(null, "horn_note_enum", new[] { "purple", "white", "yellow", "red", "orange", "green", "blue", "light_blue" })
                .ForNpgsqlHasEnum(null, "weapon_effect_enum", new[] { "fire", "water", "ice", "thunder", "dragon", "poison", "para", "sleep", "blast" })
                .ForNpgsqlHasEnum(null, "weapon_type_enum", new[] { "gs", "ls", "sns", "db", "hammer", "hh", "lance", "gl", "sa", "ig", "cb", "lbg", "hbg", "bow" })
                .HasPostgresExtension("fuzzystrmatch")
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<BowStats>(entity =>
            {
                entity.ToTable("bow_stats");

                entity.HasIndex(e => e.WeaponLevelId)
                    .HasName("bow_stats_weapon_level_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ArcShot)
                    .IsRequired()
                    .HasColumnName("arc_shot");

                entity.Property(e => e.ChargeShots)
                    .IsRequired()
                    .HasColumnName("charge_shots");

                entity.Property(e => e.Coatings)
                    .IsRequired()
                    .HasColumnName("coatings");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.BowStats)
                    .HasForeignKey<BowStats>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bow_stats_weapon_level_id_fkey");
            });

            modelBuilder.Entity<GunCrouchingFireShots>(entity =>
            {
                entity.ToTable("gun_crouching_fire_shots");

                entity.HasIndex(e => new { e.GunStatsId, e.Name })
                    .HasName("gun_crouching_fire_shots_gun_stats_id_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClipSize).HasColumnName("clip_size");

                entity.Property(e => e.GunStatsId).HasColumnName("gun_stats_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunCrouchingFireShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gun_crouching_fire_shots_gun_stats_id_fkey");
            });

            modelBuilder.Entity<GunInternalShots>(entity =>
            {
                entity.ToTable("gun_internal_shots");

                entity.HasIndex(e => new { e.GunStatsId, e.Name })
                    .HasName("gun_internal_shots_gun_stats_id_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.ClipSize).HasColumnName("clip_size");

                entity.Property(e => e.GunStatsId).HasColumnName("gun_stats_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunInternalShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gun_internal_shots_gun_stats_id_fkey");
            });

            modelBuilder.Entity<GunRapidFireShots>(entity =>
            {
                entity.ToTable("gun_rapid_fire_shots");

                entity.HasIndex(e => new { e.GunStatsId, e.Name })
                    .HasName("gun_rapid_fire_shots_gun_stats_id_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GunStatsId).HasColumnName("gun_stats_id");

                entity.Property(e => e.Modifier).HasColumnName("modifier");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.ShotCount).HasColumnName("shot_count");

                entity.Property(e => e.WaitTime).HasColumnName("wait_time");

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunRapidFireShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gun_rapid_fire_shots_gun_stats_id_fkey");
            });

            modelBuilder.Entity<GunShots>(entity =>
            {
                entity.ToTable("gun_shots");

                entity.HasIndex(e => new { e.GunStatsId, e.Name })
                    .HasName("gun_shots_gun_stats_id_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClipSize).HasColumnName("clip_size");

                entity.Property(e => e.GunStatsId).HasColumnName("gun_stats_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.NeedsSkill).HasColumnName("needs_skill");

                entity.HasOne(d => d.GunStats)
                    .WithMany(p => p.GunShots)
                    .HasForeignKey(d => d.GunStatsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gun_shots_gun_stats_id_fkey");
            });

            modelBuilder.Entity<GunStats>(entity =>
            {
                entity.ToTable("gun_stats");

                entity.HasIndex(e => e.WeaponLevelId)
                    .HasName("gun_stats_weapon_level_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Deviation)
                    .IsRequired()
                    .HasColumnName("deviation");

                entity.Property(e => e.Recoil)
                    .IsRequired()
                    .HasColumnName("recoil");

                entity.Property(e => e.ReloadSpeed)
                    .IsRequired()
                    .HasColumnName("reload_speed");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.GunStats)
                    .HasForeignKey<GunStats>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gun_stats_weapon_level_id_fkey");
            });

            modelBuilder.Entity<GunlanceShells>(entity =>
            {
                entity.ToTable("gunlance_shells");

                entity.HasIndex(e => e.WeaponLevelId)
                    .HasName("gunlance_shells_weapon_level_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ShellLevel).HasColumnName("shell_level");

                entity.Property(e => e.ShellType)
                    .IsRequired()
                    .HasColumnName("shell_type");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.GunlanceShells)
                    .HasForeignKey<GunlanceShells>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gunlance_shells_weapon_level_id_fkey");
            });

            modelBuilder.Entity<Hitzones>(entity =>
            {
                entity.ToTable("hitzones");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cut).HasColumnName("cut");

                entity.Property(e => e.Dragon).HasColumnName("dragon");

                entity.Property(e => e.Fire).HasColumnName("fire");

                entity.Property(e => e.Ice).HasColumnName("ice");

                entity.Property(e => e.Impact).HasColumnName("impact");

                entity.Property(e => e.MonsterId).HasColumnName("monster_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Shot).HasColumnName("shot");

                entity.Property(e => e.Thunder).HasColumnName("thunder");

                entity.Property(e => e.Water).HasColumnName("water");

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.Hitzones)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("hitzones_monster_id_fkey");
            });

            modelBuilder.Entity<HornNotes>(entity =>
            {
                entity.ToTable("horn_notes");

                entity.HasIndex(e => e.WeaponLevelId)
                    .HasName("horn_notes_weapon_level_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.HornNotes)
                    .HasForeignKey<HornNotes>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("horn_notes_weapon_level_id_fkey");
            });

            modelBuilder.Entity<ItemEffects>(entity =>
            {
                entity.ToTable("item_effects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Enraged).HasColumnName("enraged");

                entity.Property(e => e.Fatigued).HasColumnName("fatigued");

                entity.Property(e => e.MonsterId).HasColumnName("monster_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Normal).HasColumnName("normal");

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.ItemEffects)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("item_effects_monster_id_fkey");
            });

            modelBuilder.Entity<Monsters>(entity =>
            {
                entity.ToTable("monsters");

                entity.HasIndex(e => e.WebName)
                    .HasName("monsters_web_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ProperName)
                    .IsRequired()
                    .HasColumnName("proper_name");

                entity.Property(e => e.WebName)
                    .IsRequired()
                    .HasColumnName("web_name");
            });

            modelBuilder.Entity<Phials>(entity =>
            {
                entity.ToTable("phials");

                entity.HasIndex(e => e.WeaponLevelId)
                    .HasName("phials_weapon_level_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PhialType)
                    .IsRequired()
                    .HasColumnName("phial_type");

                entity.Property(e => e.PhialValue).HasColumnName("phial_value");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithOne(p => p.Phials)
                    .HasForeignKey<Phials>(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("phials_weapon_level_id_fkey");
            });

            modelBuilder.Entity<StaggerLimitsCommon>(entity =>
            {
                entity.ToTable("stagger_limits_common");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ExtractColour)
                    .IsRequired()
                    .HasColumnName("extract_colour");

                entity.Property(e => e.MonsterId).HasColumnName("monster_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Stagger).HasColumnName("stagger");

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.StaggerLimitsCommon)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stagger_limits_common_monster_id_fkey");
            });

            modelBuilder.Entity<StaggerLimitsGen>(entity =>
            {
                entity.ToTable("stagger_limits_gen");

                entity.HasIndex(e => e.CommonId)
                    .HasName("stagger_limits_gen_common_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CommonId).HasColumnName("common_id");

                entity.Property(e => e.Sever).HasColumnName("sever");

                entity.HasOne(d => d.Common)
                    .WithOne(p => p.StaggerLimitsGen)
                    .HasForeignKey<StaggerLimitsGen>(d => d.CommonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stagger_limits_gen_common_id_fkey");
            });

            modelBuilder.Entity<StaggerLimitsWorld>(entity =>
            {
                entity.ToTable("stagger_limits_world");

                entity.HasIndex(e => e.CommonId)
                    .HasName("stagger_limits_world_common_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Break).HasColumnName("break");

                entity.Property(e => e.CommonId).HasColumnName("common_id");

                entity.Property(e => e.Sever).HasColumnName("sever");

                entity.HasOne(d => d.Common)
                    .WithOne(p => p.StaggerLimitsWorld)
                    .HasForeignKey<StaggerLimitsWorld>(d => d.CommonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stagger_limits_world_common_id_fkey");
            });

            modelBuilder.Entity<StatusEffects>(entity =>
            {
                entity.ToTable("status_effects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Damage).HasColumnName("damage");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.Increase).HasColumnName("increase");

                entity.Property(e => e.InitialThreshold).HasColumnName("initial_threshold");

                entity.Property(e => e.MaxThreshold).HasColumnName("max_threshold");

                entity.Property(e => e.MonsterId).HasColumnName("monster_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.ReductionAmount).HasColumnName("reduction_amount");

                entity.Property(e => e.ReductionTime).HasColumnName("reduction_time");

                entity.HasOne(d => d.Monster)
                    .WithMany(p => p.StatusEffects)
                    .HasForeignKey(d => d.MonsterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("status_effects_monster_id_fkey");
            });

            modelBuilder.Entity<WeaponEffects>(entity =>
            {
                entity.ToTable("weapon_effects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attack).HasColumnName("attack");

                entity.Property(e => e.NeedsAwaken).HasColumnName("needs_awaken");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.HasOne(d => d.WeaponLevel)
                    .WithMany(p => p.WeaponEffects)
                    .HasForeignKey(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapon_effects_weapon_level_id_fkey");
            });

            modelBuilder.Entity<WeaponLevels4u>(entity =>
            {
                entity.ToTable("weapon_levels_4u");

                entity.HasIndex(e => e.CommonId)
                    .HasName("weapon_levels_4u_common_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CommonId).HasColumnName("common_id");

                entity.Property(e => e.FrenzyAffinity).HasColumnName("frenzy_affinity");

                entity.Property(e => e.Modifier).HasColumnName("modifier");

                entity.HasOne(d => d.Common)
                    .WithOne(p => p.WeaponLevels4u)
                    .HasForeignKey<WeaponLevels4u>(d => d.CommonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapon_levels_4u_common_id_fkey");
            });

            modelBuilder.Entity<WeaponLevelsCommon>(entity =>
            {
                entity.ToTable("weapon_levels_common");

                entity.HasIndex(e => new { e.WeaponId, e.LevelOrdinal })
                    .HasName("weapon_levels_common_weapon_id_level_ordinal_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Affinity).HasColumnName("affinity");

                entity.Property(e => e.Defense).HasColumnName("defense");

                entity.Property(e => e.LevelOrdinal).HasColumnName("level_ordinal");

                entity.Property(e => e.Raw).HasColumnName("raw");

                entity.Property(e => e.Slots)
                    .IsRequired()
                    .HasColumnName("slots");

                entity.Property(e => e.WeaponId).HasColumnName("weapon_id");

                entity.HasOne(d => d.Weapon)
                    .WithMany(p => p.WeaponLevelsCommon)
                    .HasForeignKey(d => d.WeaponId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapon_levels_common_weapon_id_fkey");
            });

            modelBuilder.Entity<WeaponLevelsWorld>(entity =>
            {
                entity.ToTable("weapon_levels_world");

                entity.HasIndex(e => e.CommonId)
                    .HasName("weapon_levels_world_common_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CommonId).HasColumnName("common_id");

                entity.Property(e => e.Modifier).HasColumnName("modifier");

                entity.HasOne(d => d.Common)
                    .WithOne(p => p.WeaponLevelsWorld)
                    .HasForeignKey<WeaponLevelsWorld>(d => d.CommonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapon_levels_world_common_id_fkey");
            });

            modelBuilder.Entity<WeaponSharpnesses>(entity =>
            {
                entity.ToTable("weapon_sharpnesses");

                entity.HasIndex(e => new { e.WeaponLevelId, e.SharpnessOrdinal })
                    .HasName("weapon_sharpnesses_weapon_level_id_sharpness_ordinal_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Blue).HasColumnName("blue");

                entity.Property(e => e.Green).HasColumnName("green");

                entity.Property(e => e.Orange).HasColumnName("orange");

                entity.Property(e => e.Purple).HasColumnName("purple");

                entity.Property(e => e.Red).HasColumnName("red");

                entity.Property(e => e.SharpnessOrdinal).HasColumnName("sharpness_ordinal");

                entity.Property(e => e.WeaponLevelId).HasColumnName("weapon_level_id");

                entity.Property(e => e.White).HasColumnName("white");

                entity.Property(e => e.Yellow).HasColumnName("yellow");

                entity.HasOne(d => d.WeaponLevel)
                    .WithMany(p => p.WeaponSharpnesses)
                    .HasForeignKey(d => d.WeaponLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapon_sharpnesses_weapon_level_id_fkey");
            });

            modelBuilder.Entity<WeaponsCommon>(entity =>
            {
                entity.ToTable("weapons_common");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Rare)
                    .IsRequired()
                    .HasColumnName("rare");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("weapons_common_parent_id_fkey");
            });

            modelBuilder.Entity<WeaponsGen>(entity =>
            {
                entity.ToTable("weapons_gen");

                entity.HasIndex(e => e.CommonId)
                    .HasName("weapons_gen_common_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CommonId).HasColumnName("common_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.FinalDescription)
                    .IsRequired()
                    .HasColumnName("final_description");

                entity.Property(e => e.FinalName)
                    .IsRequired()
                    .HasColumnName("final_name");

                entity.HasOne(d => d.Common)
                    .WithOne(p => p.WeaponsGen)
                    .HasForeignKey<WeaponsGen>(d => d.CommonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weapons_gen_common_id_fkey");
            });
        }
    }
}
