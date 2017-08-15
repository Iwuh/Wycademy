using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    /// <summary>
    /// Provides the movement values for weapons.
    /// </summary>
    public class MotionValueService
    {
        /// <summary>
        /// The names of all the motion value files.
        /// </summary>
        private readonly string[] WEAPON_FILENAMES = new string[] { "gs", "ls", "sns", "db", "lance", "gl", "hammer", "hh", "sa", "cb", "ig", "ammo", "lbg", "hbg", "bow", "prowler" };

        /// <summary>
        /// Gets the motion values for a weapon.
        /// </summary>
        /// <param name="weaponName">The weapon's name, either in its shortened form or all lowercase, with words separated by hyphens.</param>
        /// <returns>A tuple, with text being the code block(s) and splitPoint being the substring length, if applicable.</returns>
        public (string text, int? splitPoint) GetMotionValues(string weaponName)
        {
            string filePath = Path.Combine(WycademyConst.DATA_LOCATION, "gen", "mv", $"{AliasWeaponNames(weaponName)}.txt");

            int? point = null;
            bool split = false;
            var builder = new StringBuilder("```");
            foreach (var line in File.ReadAllLines(filePath, new UTF8Encoding(false)))
            {
                // 2000 - (```) x 2
                if (builder.Length + line.Length >= 1994 && !split)
                {
                    split = true;
                    // Close the first code block.
                    builder.Append("```");
                    point = builder.Length;
                    // Open the second code block.
                    builder.Append("```");
                }
                builder.AppendLine(line);
            }

            builder.AppendLine("```");

            return (builder.ToString(), point);
        }

        /// <summary>
        /// Converts alternate weapon names into their abbreviated form.
        /// </summary>
        /// <param name="original">The original input.</param>
        /// <returns>The shortened name.</returns>
        private string AliasWeaponNames(string original)
        {
            // Only convert it if the name isn't already in the preferred form.
            if (!WEAPON_FILENAMES.Contains(original))
            {
                switch (original)
                {
                    case "great-sword":
                        return "gs";

                    case "long-sword":
                        return "ls";

                    case "sword-and-shield":
                    case "sword-&-shield":
                    case "s&s":
                        return "sns";

                    case "dual-blades":
                        return "db";

                    case "gunlance":
                    case "gun-lance":
                        return "gl";

                    case "hunting-horn":
                        return "hh";

                    case "switch-axe":
                    case "switch_axe":
                    case "switchaxe":
                        return "sa";

                    case "charge-blade":
                    case "chargeblade":
                        return "cb";

                    case "insect-glaive":
                    case "insectglaive":
                        return "ig";

                    case "light-bowgun":
                        return "lbg";

                    case "heavy-bowgun":
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
