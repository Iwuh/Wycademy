using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfoBuilder
    {
        private StringBuilder _currentPage;
        private List<string> _pages;

        public WeaponInfoBuilder()
        {
            _currentPage = new StringBuilder();
            _pages = new List<string>();
        }

        public List<string> Build(WeaponInfo info, WeaponInfo upgradeFrom = null)
        {
            _currentPage.Clear();
            _pages.Clear();

            // Add name, weapon type, rarity, max level, what it upgrades from, and its special ability (if any).
            AddLine($"{info.Name} - {info.Weapon} (RARE {(info.IsDeviant ? "X" : info.RareLevel.ToString())})");
            AddLine($"Max level: {info.MaxLevel} / Upgrades from: {(upgradeFrom == null ? "None" : GetUpgradeFromString(info.UpgradesFromLevel.Value, upgradeFrom))} / Special Ability: {info.SpecialAbility}");
            AddLine();

            // Add in progress name, final name, description, and final description.
            var en = info.Strings.First(s => s.Language == "en");
            AddLine($"Name: {en.Name}");
            AddLine($"Final Name: {en.FinalName}");
            AddLine($"Description: {en.Description}");
            AddLine($"Final Description: {en.FinalDescription}");
            ForceNewPage();

            // Add generic level info + special info depending on the weapon type.
            foreach (WeaponLevel level in info.Levels)
            {
                AddLine($"Level {level.Level}: {level.RawDamage} / {GetElementString(level)} / {level.Affinity}%");
                //AddLine($"+{level.DefenseBoost} Defense / {level.Sharpness[0].ToString()} sharpness / {GetSlotsString(level)} / {level.Price}z");
                if (level.DefenseBoost > 0)
                {
                    // Only add the defense boost if there actually is one.
                    AddLine($"+{level.DefenseBoost} defense");
                }
                if (level.Sharpness.Count > 0)
                {
                    // Only add sharpness if the weapon is blademaster.
                    AddLine($"Sharpnesses: {level.Sharpness[0]} (base) / {level.Sharpness[1]} (+1) / {level.Sharpness[2]} (+2)");
                }
                AddLine($"{GetSlotsString(level)} / {level.Price}z");
                // Add info that's only on certain weapon types.
                AddWeaponSpecificInfo(level, info.Weapon);
                ForceNewPage();
            }

            return _pages;
        }

        private string GetUpgradeFromString(int level, WeaponInfo original)
        {
            var en = original.Strings.First(s => s.Language == "en");
            return $"{(original.MaxLevel == level ? en.FinalName : en.Name)} lv{level}";
        }

        private string GetElementString(WeaponLevel level)
        {
            if (level.Elements.Count == 0)
            {
                return "None";
            }
            else
            {
                return string.Join("&", level.Elements.Select(e => e.ToString()));
            }
        }

        private string GetSlotsString(WeaponLevel level)
        {
            return new string('O', level.Slots) + new string('-', 3 - level.Slots);
        }

        private void AddWeaponSpecificInfo(WeaponLevel level, WeaponType weapon)
        {
            switch (weapon)
            {
                case WeaponType.HBG:
                    AddLine("Usable Shots:");
                    AddLine(string.Join(" ", level.GunShots.Where(s => s.Enabled).Select(s => s.ToString())));
                    AddLine("Internal Shots:");
                    AddLine(string.Join(" ", level.InternalShots.Select(s => s.ToString())));
                    AddLine("Crouching Fire shots:");
                    AddLine(string.Join(" ", level.CrouchingFireShots.Select(s => s.ToString())));
                    AddLine($"Stats: {level.GunStats.ReloadSpeed} Reload Speed / {level.GunStats.Recoil} Recoil / {level.GunStats.Deviation} Deviation");
                    break;

                case WeaponType.LBG:
                    AddLine("Usable Shots:");
                    AddLine(string.Join(" ", level.GunShots.Where(s => s.Enabled).Select(s => s.ToString())));
                    AddLine("Internal Shots:");
                    AddLine(string.Join(" ", level.InternalShots.Select(s => s.ToString())));
                    AddLine("Rapidfire shots:");
                    AddLine(string.Join(" ", level.RapidfireShots.Select(s => s.ToString())));
                    AddLine($"Stats: {level.GunStats.ReloadSpeed} Reload Speed / {level.GunStats.Recoil} Recoil / {level.GunStats.Deviation} Deviation");
                    break;

                case WeaponType.SA:
                    AddLine(level.SACBPhials[0].ToString());
                    break;

                case WeaponType.GL:
                    AddLine(level.GLShells[0].ToString());
                    break;

                case WeaponType.Bow:
                    AddLine(string.Join(" ", level.BowCoatings.Select(c => c.ToString())));
                    break;

                case WeaponType.HH:
                    AddLine(level.HHNotes[0].ToString());
                    break;

                case WeaponType.CB:
                    AddLine(level.SACBPhials[0].ToString());
                    break;
            }
        }

        private void AddLine(string line)
        {
            // If the line would bring the stringbuilder over 1950 chars (2000 is the limit) make a new page.
            if (_currentPage.Length + line.Length >= 1950)
            {
                _pages.Add(_currentPage.ToString());
                _currentPage.Clear();
                _currentPage.AppendLine(line);
            }
            else
            {
                _currentPage.AppendLine(line);
            }
        }

        private void AddLine()
        {
            _currentPage.AppendLine();
        }

        private void ForceNewPage()
        {
            _pages.Add(_currentPage.ToString());
            _currentPage.Clear();
        }
    }
}
