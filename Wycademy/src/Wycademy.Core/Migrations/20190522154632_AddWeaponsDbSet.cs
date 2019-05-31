using Microsoft.EntityFrameworkCore.Migrations;

namespace Wycademy.Core.Migrations
{
    public partial class AddWeaponsDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GunlanceShells_WeaponLevels_WeaponLevelId",
                table: "GunlanceShells");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_Weapon_ParentId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponLevels_Weapon_WeaponId",
                table: "WeaponLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weapon",
                table: "Weapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GunlanceShells",
                table: "GunlanceShells");

            migrationBuilder.RenameTable(
                name: "Weapon",
                newName: "Weapons");

            migrationBuilder.RenameTable(
                name: "GunlanceShells",
                newName: "GunlanceShellStats");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_Game_Name_WeaponType",
                table: "Weapons",
                newName: "IX_Weapons_Game_Name_WeaponType");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_ParentId",
                table: "Weapons",
                newName: "IX_Weapons_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_GunlanceShells_WeaponLevelId",
                table: "GunlanceShellStats",
                newName: "IX_GunlanceShellStats_WeaponLevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weapons",
                table: "Weapons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GunlanceShellStats",
                table: "GunlanceShellStats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GunlanceShellStats_WeaponLevels_WeaponLevelId",
                table: "GunlanceShellStats",
                column: "WeaponLevelId",
                principalTable: "WeaponLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponLevels_Weapons_WeaponId",
                table: "WeaponLevels",
                column: "WeaponId",
                principalTable: "Weapons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Weapons_ParentId",
                table: "Weapons",
                column: "ParentId",
                principalTable: "Weapons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GunlanceShellStats_WeaponLevels_WeaponLevelId",
                table: "GunlanceShellStats");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponLevels_Weapons_WeaponId",
                table: "WeaponLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Weapons_ParentId",
                table: "Weapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weapons",
                table: "Weapons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GunlanceShellStats",
                table: "GunlanceShellStats");

            migrationBuilder.RenameTable(
                name: "Weapons",
                newName: "Weapon");

            migrationBuilder.RenameTable(
                name: "GunlanceShellStats",
                newName: "GunlanceShells");

            migrationBuilder.RenameIndex(
                name: "IX_Weapons_Game_Name_WeaponType",
                table: "Weapon",
                newName: "IX_Weapon_Game_Name_WeaponType");

            migrationBuilder.RenameIndex(
                name: "IX_Weapons_ParentId",
                table: "Weapon",
                newName: "IX_Weapon_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_GunlanceShellStats_WeaponLevelId",
                table: "GunlanceShells",
                newName: "IX_GunlanceShells_WeaponLevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weapon",
                table: "Weapon",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GunlanceShells",
                table: "GunlanceShells",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GunlanceShells_WeaponLevels_WeaponLevelId",
                table: "GunlanceShells",
                column: "WeaponLevelId",
                principalTable: "WeaponLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_Weapon_ParentId",
                table: "Weapon",
                column: "ParentId",
                principalTable: "Weapon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponLevels_Weapon_WeaponId",
                table: "WeaponLevels",
                column: "WeaponId",
                principalTable: "Weapon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
