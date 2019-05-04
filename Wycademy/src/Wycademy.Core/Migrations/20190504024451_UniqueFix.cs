using Microsoft.EntityFrameworkCore.Migrations;

namespace Wycademy.Core.Migrations
{
    public partial class UniqueFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WeaponSharpnesses_WeaponLevelId_SharpnessOrdinal",
                table: "WeaponSharpnesses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_WeaponLevels_WeaponId_LevelOrdinal",
                table: "WeaponLevels");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_WeaponEffects_WeaponLevelId_EffectType",
                table: "WeaponEffects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Weapon_Game_Name_WeaponType",
                table: "Weapon");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StatusEffects_MonsterId_Game_Name",
                table: "StatusEffects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StaggerLimits_MonsterId_Game_Name",
                table: "StaggerLimits");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ItemEffects_MonsterId_Game_Name",
                table: "ItemEffects");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Hitzones_MonsterId_Game_Name",
                table: "Hitzones");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GunShots_GunStatsId_Name",
                table: "GunShots");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GunRapidFireShots_GunStatsId_Name",
                table: "GunRapidFireShots");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GunInternalShots_GunStatsId_Name",
                table: "GunInternalShots");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GunCrouchingFireShots_GunStatsId_Name",
                table: "GunCrouchingFireShots");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponSharpnesses_WeaponLevelId_SharpnessOrdinal",
                table: "WeaponSharpnesses",
                columns: new[] { "WeaponLevelId", "SharpnessOrdinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponLevels_WeaponId_LevelOrdinal",
                table: "WeaponLevels",
                columns: new[] { "WeaponId", "LevelOrdinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponEffects_WeaponLevelId_EffectType",
                table: "WeaponEffects",
                columns: new[] { "WeaponLevelId", "EffectType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_Game_Name_WeaponType",
                table: "Weapon",
                columns: new[] { "Game", "Name", "WeaponType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusEffects_MonsterId_Game_Name",
                table: "StatusEffects",
                columns: new[] { "MonsterId", "Game", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaggerLimits_MonsterId_Game_Name",
                table: "StaggerLimits",
                columns: new[] { "MonsterId", "Game", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemEffects_MonsterId_Game_Name",
                table: "ItemEffects",
                columns: new[] { "MonsterId", "Game", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hitzones_MonsterId_Game_Name",
                table: "Hitzones",
                columns: new[] { "MonsterId", "Game", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunShots_GunStatsId_Name",
                table: "GunShots",
                columns: new[] { "GunStatsId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunRapidFireShots_GunStatsId_Name",
                table: "GunRapidFireShots",
                columns: new[] { "GunStatsId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunInternalShots_GunStatsId_Name",
                table: "GunInternalShots",
                columns: new[] { "GunStatsId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GunCrouchingFireShots_GunStatsId_Name",
                table: "GunCrouchingFireShots",
                columns: new[] { "GunStatsId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeaponSharpnesses_WeaponLevelId_SharpnessOrdinal",
                table: "WeaponSharpnesses");

            migrationBuilder.DropIndex(
                name: "IX_WeaponLevels_WeaponId_LevelOrdinal",
                table: "WeaponLevels");

            migrationBuilder.DropIndex(
                name: "IX_WeaponEffects_WeaponLevelId_EffectType",
                table: "WeaponEffects");

            migrationBuilder.DropIndex(
                name: "IX_Weapon_Game_Name_WeaponType",
                table: "Weapon");

            migrationBuilder.DropIndex(
                name: "IX_StatusEffects_MonsterId_Game_Name",
                table: "StatusEffects");

            migrationBuilder.DropIndex(
                name: "IX_StaggerLimits_MonsterId_Game_Name",
                table: "StaggerLimits");

            migrationBuilder.DropIndex(
                name: "IX_ItemEffects_MonsterId_Game_Name",
                table: "ItemEffects");

            migrationBuilder.DropIndex(
                name: "IX_Hitzones_MonsterId_Game_Name",
                table: "Hitzones");

            migrationBuilder.DropIndex(
                name: "IX_GunShots_GunStatsId_Name",
                table: "GunShots");

            migrationBuilder.DropIndex(
                name: "IX_GunRapidFireShots_GunStatsId_Name",
                table: "GunRapidFireShots");

            migrationBuilder.DropIndex(
                name: "IX_GunInternalShots_GunStatsId_Name",
                table: "GunInternalShots");

            migrationBuilder.DropIndex(
                name: "IX_GunCrouchingFireShots_GunStatsId_Name",
                table: "GunCrouchingFireShots");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WeaponSharpnesses_WeaponLevelId_SharpnessOrdinal",
                table: "WeaponSharpnesses",
                columns: new[] { "WeaponLevelId", "SharpnessOrdinal" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WeaponLevels_WeaponId_LevelOrdinal",
                table: "WeaponLevels",
                columns: new[] { "WeaponId", "LevelOrdinal" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WeaponEffects_WeaponLevelId_EffectType",
                table: "WeaponEffects",
                columns: new[] { "WeaponLevelId", "EffectType" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Weapon_Game_Name_WeaponType",
                table: "Weapon",
                columns: new[] { "Game", "Name", "WeaponType" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StatusEffects_MonsterId_Game_Name",
                table: "StatusEffects",
                columns: new[] { "MonsterId", "Game", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StaggerLimits_MonsterId_Game_Name",
                table: "StaggerLimits",
                columns: new[] { "MonsterId", "Game", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ItemEffects_MonsterId_Game_Name",
                table: "ItemEffects",
                columns: new[] { "MonsterId", "Game", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Hitzones_MonsterId_Game_Name",
                table: "Hitzones",
                columns: new[] { "MonsterId", "Game", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GunShots_GunStatsId_Name",
                table: "GunShots",
                columns: new[] { "GunStatsId", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GunRapidFireShots_GunStatsId_Name",
                table: "GunRapidFireShots",
                columns: new[] { "GunStatsId", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GunInternalShots_GunStatsId_Name",
                table: "GunInternalShots",
                columns: new[] { "GunStatsId", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GunCrouchingFireShots_GunStatsId_Name",
                table: "GunCrouchingFireShots",
                columns: new[] { "GunStatsId", "Name" });
        }
    }
}
