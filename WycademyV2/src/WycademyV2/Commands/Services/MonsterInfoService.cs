using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;

namespace WycademyV2.Commands.Services
{
    /// <summary>
    /// Provides data about monsters.
    /// </summary>
    public class MonsterInfoService
    {
        /// <summary>
        /// Used for statistics.
        /// </summary>
        public int Queries { get; private set; }

        private readonly string[] HITZONE_COLUMN_NAMES = new string[] { "Cut", "Impact", "Shot", "Fire", "Water", "Ice", "Thunder", "Dragon" };
        private readonly string[] STAGGER_COLUMN_NAMES = new string[] { "Stagger Value", "Sever Value", "Extract Colour" };
        private readonly string[] STATUS_COLUMN_NAMES = new string[] { "Initial", "Increase", "Max", "Duration", "Reduction", "Damage" };
        private readonly string[] ITEMEFFECTS_COLUMN_NAMES = new string[] { "Duration Normal", "Duration Enraged", "Duration Fatigued" };
        private readonly string[] MONSTER_LIST = new string[] 
        {
            "great-maccao",
            "velocidrome",
            "bulldrome",
            "seltas",
            "arzuros",
            "redhelm-arzuros",
            "gendrome",
            "cephadrome",
            "yian-kut-ku",
            "iodrome",
            "kecha-wacha",
            "lagombi",
            "snowbaron-lagombi",
            "gypceros",
            "tetsucabra",
            "drilltusk-tetsucabra",
            "daimyo-hermitaur",
            "stonefist-hermitaur",
            "volvidon",
            "royal-ludroth",
            "malfestio",
            "zamtrios",
            "khezu",
            "rathian",
            "gold-rathian",
            "dreadqueen-rathian",
            "nibelsnarf",
            "plesioth",
            "blangonga",
            "lavasioth",
            "shogun-ceanataur",
            "najarala",
            "nargacuga",
            "silverwind-nargacuga",
            "yian-garuga",
            "deadeye-yian-garuga",
            "uragaan",
            "crystalbeard-uragaan",
            "seltas-queen",
            "rathalos",
            "silver-rathalos",
            "dreadking-rathalos",
            "lagiacrus",
            "zinogre",
            "thunderlord-zinogre",
            "mizutsune",
            "astalos",
            "gammoth",
            "glavenus",
            "hellblade-glavenus",
            "agnaktor",
            "gore-magala",
            "seregios",
            "duramboros",
            "tigrex",
            "grimclaw-tigrex",
            "kirin",
            "brachydios",
            "shagaru-magala",
            "rajang",
            "furious-rajang",
            "deviljho",
            "savage-deviljho",
            "kushala-daora",
            "chameleos",
            "teostra",
            "akantor",
            "ukanlos",
            "amatsu",
            "nakarkos",
            "alatreon"
        };

        /// <summary>
        /// Gets a table containing the requested data about the requested monster.
        /// </summary>
        /// <param name="category">The category of data to find.</param>
        /// <param name="monsterName">The name of the monster to find data for.</param>
        /// <returns>Task(string)</returns>
        public string GetMonsterInfo(string category, string monsterName)
        {
#if DEBUG
            string path = Path.Combine(WycademyConst.DATA_LOCATION, "gen", "monster", $"{monsterName}.json");
#else
            string path = Path.Combine("Data", "gen", "monster", $"{monsterName}".json);
#endif

            // Deserialise the json into a MonsterInfo object.
            MonsterInfo monster = JsonConvert.DeserializeObject<MonsterInfo>(File.ReadAllText(path, Encoding.UTF8));

            StringBuilder infoBuilder = new StringBuilder();
            string[] columnNames;
            Dictionary<string, List<string>> data;

            // Get the appropriate column titles depending on the category.
            switch (category)
            {
                case "Hitzone":
                    columnNames = HITZONE_COLUMN_NAMES;
                    data = monster.Hitzones;
                    break;
                case "Status":
                    columnNames = STATUS_COLUMN_NAMES;
                    data = monster.Status;
                    break;
                case "Stagger":
                    columnNames = STAGGER_COLUMN_NAMES;
                    data = monster.Stagger;
                    break;
                case "Item Effect":
                    columnNames = ITEMEFFECTS_COLUMN_NAMES;
                    data = monster.Items;
                    break;
                default:
                    throw new ArgumentException($"{category} is not a valid category.");
            }
            // Set the widths of the row title column and the data columns.
            int columnTitleWidth = columnNames.Max(x => x.Length);
            int rowTitleWidth = data.Keys.Max(x => x.Length);

            // Add a title to the table.
            infoBuilder.AppendLine($"{category} info for {GetFormattedName(monsterName)}:");

            // Open the code block.
            infoBuilder.AppendLine("```");

            // Add blank space to the upper-left corner.
            infoBuilder.Append(' ', rowTitleWidth);

            // Add the column titles and a newline.
            foreach (var title in columnNames)
            {
                infoBuilder.Append("|" + PadCenter(title, columnTitleWidth));
            }
            infoBuilder.AppendLine();

            // Add rows.
            foreach (var title in data.Keys)
            {
                // Add the row title with spaces appended so that each title is the same width.
                infoBuilder.Append(title + new string(' ', rowTitleWidth - title.Length));

                foreach (var value in data[title])
                {
                    infoBuilder.Append($"|{PadCenter(value, columnTitleWidth)}");
                }
                infoBuilder.AppendLine();
            }

            // Close the code block.
            infoBuilder.AppendLine("```");

            Queries++;

            return infoBuilder.ToString();
        }

        public string GetMonsterNames()
        {
            return "```\n" + string.Join("\n", MONSTER_LIST.Select(s => GetFormattedName(s))) + "```";
        }

        /// <summary>
        /// Pads both sides of a string with spaces to get the requested total length.
        /// </summary>
        /// <param name="stringToPad">The string to pad.</param>
        /// <param name="totalLength">The length of the returned string.</param>
        /// <returns>string</returns>
        private string PadCenter(string stringToPad, int totalLength)
        {
            // Finds the total amount of spaces to pad.
            int delta = totalLength - stringToPad.Length;
            // PadLeft adds spaces to bump the character count up to the required length,
            // so this adds half of the total amount of spaces to add to the left.
            int padLeft = delta / 2 + stringToPad.Length;

            return stringToPad.PadLeft(padLeft).PadRight(totalLength);
        }

        /// <summary>
        /// Converts a monster filename to its properly formatted name (ex. gore-magala -> Gore Magala).
        /// </summary>
        /// <param name="input">The string to format.</param>
        /// <returns>The input string, formatted to title case.</returns>
        private string GetFormattedName(string input)
        {
            // The one exception, as Kut-Ku should keep the hyphen in between.
            if (input == "yian-kut-ku") return "Yian Kut-Ku";

            string[] split = input.Split('-');
            var properCase = split.Select(s => s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower());
            return string.Join(" ", properCase);
        }
    }
}
