using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    static class InfoBuilder
    {
        // Used for statistics
        public static int Queries { get; private set; }

        // Json deserialization is cpu-bound work, so commands must use Task.Run to get a MonsterInfo without blocking.
        public static MonsterInfo GetMonsterInfo(string monster, string category)
        {
            if (WycademySettings.MONSTER_LIST.Contains(monster, StringComparer.InvariantCultureIgnoreCase))
            {
                // Read the content of a json file into a JObject
                JObject parsed;
                using (StreamReader sr = new StreamReader($"{WycademySettings.JSON_PATH}\\{monster}.json"))
                {
                    parsed = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
                }

                MonsterInfo info = new MonsterInfo(category);
                info.Data = ConvertJsonFragmentToDictionary(parsed, category.ToLower());

                Queries++;
                return info;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static FileStream GetMotionValueStream(string weapon)
        {
            string file = $"{ShortenWeaponNames(weapon)}.txt";

            return File.Open(@".\..\..\Data\mv\" + file, FileMode.Open);
        }

        private static Dictionary<string, string[]> ConvertJsonFragmentToDictionary(JObject full, string subsection)
        {
            // Gets a subsection of the full json, then casts it to an IDictionary<string, JToken>.
            // We then call ToDictionary on it and use LINQ magic to convert it to a Dictionary<string, string[]>
            return ((IDictionary<string, JToken>)full[subsection]).ToDictionary(pair => pair.Key, pair => ((string)pair.Value).Split(' '));
        }

        private static string ShortenWeaponNames(string name)
        {
            if (!WycademySettings.WEAPON_NAMES.Contains(name, StringComparer.InvariantCultureIgnoreCase))
            {
                switch (name.ToLower())
                {
                    case "great sword":
                    case "great_sword":
                        return "GS";

                    case "long sword":
                    case "long_sword":
                        return "LS";

                    case "sword and shield":
                    case "sword_and_shield":
                    case "sword & shield":
                    case "s&s":
                        return "SnS";

                    case "dual blades":
                    case "dual_blades":
                        return "DB";

                    case "gunlance":
                    case "gun_lance":
                    case "gun lance":
                        return "GL";

                    case "hemr":
                    case "hemmr":
                        return "Hammer";

                    case "hunting horn":
                    case "hunting_horn":
                    case "doot":
                        return "HH";

                    case "switch axe":
                    case "switch_axe":
                    case "switchaxe":
                    case "swaxe":
                    case "swag axe":
                        return "SA";

                    case "charge blade":
                    case "charge_blade":
                    case "chargeblade":
                        return "CB";

                    case "light bowgun":
                    case "light_bowgun":
                        return "LBG";

                    case "heavy bowgun":
                    case "heavy_bowgun":
                        return "HBG";

                    default:
                        throw new ArgumentException();
                }
            }

            return name;
        }
    }
}
