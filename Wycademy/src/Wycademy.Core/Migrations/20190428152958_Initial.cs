using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:game", "four,generations,world")
                .Annotation("Npgsql:Enum:horn_note", "purple,white,yellow,red,orange,green,blue,light_blue")
                .Annotation("Npgsql:Enum:weapon_effect_type", "fire,water,ice,thunder,dragon,poison,para,sleep,blast")
                .Annotation("Npgsql:Enum:weapon_type", "gs,ls,sn_s,db,hammer,hh,lance,gl,sa,ig,cb,lbg,hbg,bow")
                .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,");

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WebName = table.Column<string>(nullable: false),
                    ProperName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                    table.UniqueConstraint("AK_Monsters_WebName", x => x.WebName);
                });

            migrationBuilder.CreateTable(
                name: "Weapon",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Game = table.Column<Game>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    WeaponType = table.Column<WeaponType>(nullable: false),
                    Rare = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    FinalName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FinalDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.Id);
                    table.UniqueConstraint("AK_Weapon_Game_Name_WeaponType", x => new { x.Game, x.Name, x.WeaponType });
                    table.ForeignKey(
                        name: "FK_Weapon_Weapon_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Weapon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hitzones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(nullable: false),
                    Game = table.Column<Game>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Cut = table.Column<int>(nullable: false),
                    Impact = table.Column<int>(nullable: false),
                    Shot = table.Column<int>(nullable: false),
                    Fire = table.Column<int>(nullable: false),
                    Water = table.Column<int>(nullable: false),
                    Ice = table.Column<int>(nullable: false),
                    Thunder = table.Column<int>(nullable: false),
                    Dragon = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hitzones", x => x.Id);
                    table.UniqueConstraint("AK_Hitzones_MonsterId_Game_Name", x => new { x.MonsterId, x.Game, x.Name });
                    table.ForeignKey(
                        name: "FK_Hitzones_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(nullable: false),
                    Game = table.Column<Game>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Normal = table.Column<int>(nullable: false),
                    Enraged = table.Column<int>(nullable: false),
                    Fatigued = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEffects", x => x.Id);
                    table.UniqueConstraint("AK_ItemEffects_MonsterId_Game_Name", x => new { x.MonsterId, x.Game, x.Name });
                    table.ForeignKey(
                        name: "FK_ItemEffects_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaggerLimits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(nullable: false),
                    Game = table.Column<Game>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Stagger = table.Column<int>(nullable: false),
                    ExtractColour = table.Column<string>(nullable: false),
                    Sever = table.Column<int>(nullable: true),
                    Break = table.Column<int>(nullable: true),
                    StaggerLimitWorld_Sever = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaggerLimits", x => x.Id);
                    table.UniqueConstraint("AK_StaggerLimits_MonsterId_Game_Name", x => new { x.MonsterId, x.Game, x.Name });
                    table.ForeignKey(
                        name: "FK_StaggerLimits_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatusEffects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(nullable: false),
                    Game = table.Column<Game>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    InitialThreshold = table.Column<int>(nullable: false),
                    Increase = table.Column<int>(nullable: false),
                    MaxThreshold = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    ReductionTime = table.Column<int>(nullable: false),
                    ReductionAmount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEffects", x => x.Id);
                    table.UniqueConstraint("AK_StatusEffects_MonsterId_Game_Name", x => new { x.MonsterId, x.Game, x.Name });
                    table.ForeignKey(
                        name: "FK_StatusEffects_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponLevels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponId = table.Column<int>(nullable: false),
                    Game = table.Column<Game>(nullable: false),
                    LevelOrdinal = table.Column<int>(nullable: false),
                    Raw = table.Column<int>(nullable: false),
                    Affinity = table.Column<int>(nullable: false),
                    Defense = table.Column<int>(nullable: false),
                    Slots = table.Column<string>(nullable: false),
                    DisplayModifier = table.Column<float>(nullable: true),
                    FrenzyAffinity = table.Column<int>(nullable: true),
                    WeaponLevelWorld_DisplayModifier = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponLevels", x => x.Id);
                    table.UniqueConstraint("AK_WeaponLevels_WeaponId_LevelOrdinal", x => new { x.WeaponId, x.LevelOrdinal });
                    table.ForeignKey(
                        name: "FK_WeaponLevels_Weapon_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BowStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    ArcShot = table.Column<string>(nullable: false),
                    ChargeShots = table.Column<string[]>(nullable: false),
                    Coatings = table.Column<string[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BowStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BowStats_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunlanceShells",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    ShellType = table.Column<string>(nullable: false),
                    ShellLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunlanceShells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GunlanceShells_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    ReloadSpeed = table.Column<string>(nullable: false),
                    Recoil = table.Column<string>(nullable: false),
                    Deviation = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GunStats_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HornNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    Note1 = table.Column<HornNote>(nullable: false),
                    Note2 = table.Column<HornNote>(nullable: false),
                    Note3 = table.Column<HornNote>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HornNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HornNotes_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    PhialType = table.Column<string>(nullable: false),
                    PhialValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phials_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponEffects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    EffectType = table.Column<WeaponEffectType>(nullable: false),
                    Attack = table.Column<int>(nullable: false),
                    NeedsAwaken = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponEffects", x => x.Id);
                    table.UniqueConstraint("AK_WeaponEffects_WeaponLevelId_EffectType", x => new { x.WeaponLevelId, x.EffectType });
                    table.ForeignKey(
                        name: "FK_WeaponEffects_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponSharpnesses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponLevelId = table.Column<int>(nullable: false),
                    SharpnessOrdinal = table.Column<int>(nullable: false),
                    Red = table.Column<int>(nullable: false),
                    Orange = table.Column<int>(nullable: false),
                    Yellow = table.Column<int>(nullable: false),
                    Green = table.Column<int>(nullable: false),
                    Blue = table.Column<int>(nullable: false),
                    White = table.Column<int>(nullable: false),
                    Purple = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponSharpnesses", x => x.Id);
                    table.UniqueConstraint("AK_WeaponSharpnesses_WeaponLevelId_SharpnessOrdinal", x => new { x.WeaponLevelId, x.SharpnessOrdinal });
                    table.ForeignKey(
                        name: "FK_WeaponSharpnesses_WeaponLevels_WeaponLevelId",
                        column: x => x.WeaponLevelId,
                        principalTable: "WeaponLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunCrouchingFireShots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GunStatsId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ClipSize = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunCrouchingFireShots", x => x.Id);
                    table.UniqueConstraint("AK_GunCrouchingFireShots_GunStatsId_Name", x => new { x.GunStatsId, x.Name });
                    table.ForeignKey(
                        name: "FK_GunCrouchingFireShots_GunStats_GunStatsId",
                        column: x => x.GunStatsId,
                        principalTable: "GunStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunInternalShots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GunStatsId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    ClipSize = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunInternalShots", x => x.Id);
                    table.UniqueConstraint("AK_GunInternalShots_GunStatsId_Name", x => new { x.GunStatsId, x.Name });
                    table.ForeignKey(
                        name: "FK_GunInternalShots_GunStats_GunStatsId",
                        column: x => x.GunStatsId,
                        principalTable: "GunStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunRapidFireShots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GunStatsId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ShotCount = table.Column<int>(nullable: false),
                    Modifier = table.Column<float>(nullable: true),
                    WaitTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunRapidFireShots", x => x.Id);
                    table.UniqueConstraint("AK_GunRapidFireShots_GunStatsId_Name", x => new { x.GunStatsId, x.Name });
                    table.ForeignKey(
                        name: "FK_GunRapidFireShots_GunStats_GunStatsId",
                        column: x => x.GunStatsId,
                        principalTable: "GunStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunShots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GunStatsId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ClipSize = table.Column<int>(nullable: false),
                    NeedsSkill = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunShots", x => x.Id);
                    table.UniqueConstraint("AK_GunShots_GunStatsId_Name", x => new { x.GunStatsId, x.Name });
                    table.ForeignKey(
                        name: "FK_GunShots_GunStats_GunStatsId",
                        column: x => x.GunStatsId,
                        principalTable: "GunStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BowStats_WeaponLevelId",
                table: "BowStats",
                column: "WeaponLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunlanceShells_WeaponLevelId",
                table: "GunlanceShells",
                column: "WeaponLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunStats_WeaponLevelId",
                table: "GunStats",
                column: "WeaponLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HornNotes_WeaponLevelId",
                table: "HornNotes",
                column: "WeaponLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phials_WeaponLevelId",
                table: "Phials",
                column: "WeaponLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_ParentId",
                table: "Weapon",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BowStats");

            migrationBuilder.DropTable(
                name: "GunCrouchingFireShots");

            migrationBuilder.DropTable(
                name: "GunInternalShots");

            migrationBuilder.DropTable(
                name: "GunlanceShells");

            migrationBuilder.DropTable(
                name: "GunRapidFireShots");

            migrationBuilder.DropTable(
                name: "GunShots");

            migrationBuilder.DropTable(
                name: "Hitzones");

            migrationBuilder.DropTable(
                name: "HornNotes");

            migrationBuilder.DropTable(
                name: "ItemEffects");

            migrationBuilder.DropTable(
                name: "Phials");

            migrationBuilder.DropTable(
                name: "StaggerLimits");

            migrationBuilder.DropTable(
                name: "StatusEffects");

            migrationBuilder.DropTable(
                name: "WeaponEffects");

            migrationBuilder.DropTable(
                name: "WeaponSharpnesses");

            migrationBuilder.DropTable(
                name: "GunStats");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "WeaponLevels");

            migrationBuilder.DropTable(
                name: "Weapon");
        }
    }
}
