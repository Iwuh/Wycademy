using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    /// <summary>
    /// Provides the movement values for weapons.
    /// </summary>
    public class MotionValueService
    {
        /// <summary>
        /// Used for statistics.
        /// </summary>
        public int Queries { get; private set; }

        /// <summary>
        /// The names of all the motion value files.
        /// </summary>
        private readonly string[] WEAPON_FILENAMES = new string[] { "GS", "LS", "SnS", "DB", "Lance", "GL", "Hammer", "HH", "SA", "CB", "Ammo", "LBG", "HBG", "Bow", "Prowler" };

        /// <summary>
        /// Get the file containing the movement values for a specific weapon.
        /// </summary>
        /// <param name="name">The name of the weapon to search for.</param>
        /// <returns>Task(FileStream)</returns>
        public FileStream GetMotionValueStream(string name)
        {
            string fileName = string.Join(Path.DirectorySeparatorChar.ToString(), WycademyConst.DATA_LOCATION, "mv", $"{AliasWeaponNames(name)}.txt");

            Queries++;

            return File.Open(fileName, FileMode.Open);
        }

        public string GetWeaponNames()
        {
            return "```\n" + string.Join("\n", WEAPON_FILENAMES) + "```\n" + "Other forms of these abbreviations are recognised as well.";
        }

        /// <summary>
        /// Converts alternate weapon names into their abbreviated form.
        /// </summary>
        /// <param name="original">The original input.</param>
        /// <returns>The shortened name.</returns>
        private string AliasWeaponNames(string original)
        {
            // Only convert it if the name isn't already in the preferred form.
            if (!WEAPON_FILENAMES.Contains(original, StringComparer.OrdinalIgnoreCase))
            {
                switch (original)
                {
                    case "great sword":
                    case "great_sword":
                        return "gs";

                    case "long sword":
                    case "long_sword":
                        return "ls";

                    case "sword and shield":
                    case "sword_and_shield":
                    case "sword & shield":
                    case "s&s":
                        return "sns";

                    case "dual blades":
                    case "dual_blades":
                        return "db";

                    case "gunlance":
                    case "gun_lance":
                    case "gun lance":
                        return "gl";

                    case "hemr":
                    case "hemmr":
                        return "hammer";

                    case "hunting horn":
                    case "hunting_horn":
                    case "doot":
                        return "hh";

                    case "switch axe":
                    case "switch_axe":
                    case "switchaxe":
                    case "swaxe":
                    case "swag axe":
                        return "sa";

                    case "charge blade":
                    case "charge_blade":
                    case "chargeblade":
                        return "cb";

                    case "light bowgun":
                    case "light_bowgun":
                        return "lbg";

                    case "heavy bowgun":
                    case "heavy_bowgun":
                        return "lbg";

                    default:
                        throw new ArgumentException();
                }
            }

            // If the if statement is never entered then just return the original input.
            return original;
        }
    }
}
