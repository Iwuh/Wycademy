using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly string[] MONSTER_LIST = new string[71]
        {
            "Great_Maccao",
            "Velocidrome",
            "Bulldrome",
            "Seltas",
            "Arzuros",
            "Redhelm_Arzuros",
            "Gendrome",
            "Cephadrome",
            "Yian_Kut-Ku",
            "Iodrome",
            "Kecha_Wacha",
            "Lagombi",
            "Snowbaron_Lagombi",
            "Gypceros",
            "Tetsucabra",
            "Drilltusk_Tetsucabra",
            "Daimyo_Hermitaur",
            "Stonefist_Hermitaur",
            "Volvidon",
            "Royal_Ludroth",
            "Malfestio",
            "Zamtrios",
            "Khezu",
            "Rathian",
            "Gold_Rathian",
            "Dreadqueen_Rathian",
            "Nibelsnarf",
            "Plesioth",
            "Blagonga",
            "Lavasioth",
            "Shogun_Ceanataur",
            "Najarala",
            "Nargacuga",
            "Silverwind_Nargacuga",
            "Yian_Garuga",
            "Deadeye_Yian_Garuga",
            "Uragaan",
            "Crystalbeard_Uragaan",
            "Seltas_Queen",
            "Rathalos",
            "Silver_Rathalos",
            "Dreadking_Rathalos",
            "Lagiacrus",
            "Zinogre",
            "Thunderlord_Zinogre",
            "Mizutsune",
            "Astalos",
            "Gammoth",
            "Glavenus",
            "Hellblade_Glavenus",
            "Agnaktor",
            "Gore_Magala",
            "Seregios",
            "Duramboros",
            "Tigrex",
            "Grimclaw_Tigrex",
            "Kirin",
            "Brachydios",
            "Shagaru_Magala",
            "Rajang",
            "Furious_Rajang",
            "Deviljho",
            "Savage_Deviljho",
            "Kushala_Daora",
            "Chameleos",
            "Teostra",
            "Akantor",
            "Ukanlos",
            "Amatsu",
            "Nakarkos",
            "Alatreon"
        };

        /// <summary>
        /// Gets a table containing the requested data about the requested monster.
        /// </summary>
        /// <param name="category">The category of data to find.</param>
        /// <param name="monsterName">The name of the monster to find data for.</param>
        /// <returns>Task(string)</returns>
        public async Task<string> GetMonsterInfo(string category, string monsterName)
        {
            string newName;
            var split = monsterName.Split(' ');
            if (split.Length > 1)
            {
                newName = string.Join("_", split);
            }
            else
            {
                newName = monsterName;
            }

            // Deserialise the json on a threadpool to avoid blocking while reading from disk.
            Dictionary<string, string[]> Data = await Task.Run(() => GetDictionaryFromJson(category, newName));

            StringBuilder infoBuilder = new StringBuilder();
            int columnTitleWidth;
            int rowTitleWidth;
            string[] columnNames;

            // Get the appropriate column titles depending on the category.
            switch (category)
            {
                case "Hitzone":
                    columnNames = HITZONE_COLUMN_NAMES;
                    break;
                case "Status":
                    columnNames = STATUS_COLUMN_NAMES;
                    break;
                case "Stagger/Sever":
                    columnNames = STAGGER_COLUMN_NAMES;
                    break;
                case "Item Effects":
                    columnNames = ITEMEFFECTS_COLUMN_NAMES;
                    break;
                default:
                    throw new ArgumentException($"{category} is not a valid category.");
            }
            // Set the widths of the row title column and the data columns.
            columnTitleWidth = columnNames.Max(x => x.Length);
            rowTitleWidth = Data.Keys.Max(x => x.Length);

            // Add a title to the table.
            infoBuilder.AppendLine($"{category} info for {monsterName}:");

            // Open the code block.
            infoBuilder.AppendLine("```xl");

            // Add blank space to the upper-left corner.
            infoBuilder.Append(' ', rowTitleWidth);

            // Add the column titles and a newline.
            foreach (var title in columnNames)
            {
                infoBuilder.Append("|" + PadCenter(title, columnTitleWidth));
            }
            infoBuilder.AppendLine();

            // Add rows.
            foreach (var title in Data.Keys)
            {
                // Add the row title with spaces appended so that each title is the same width.
                infoBuilder.Append(title + new string(' ', rowTitleWidth - title.Length));

                if (category == "Hitzone")
                {
                    // Each key has 10 values, but the last 2 are unused due to character limits, and I'm not editing 71 sheets.
                    for (int i = 0; i < 8; i++)
                    {
                        infoBuilder.Append($"|{PadCenter(Data[title][i], columnTitleWidth)}");
                    }
                    infoBuilder.AppendLine();
                }
                else
                {
                    foreach (var value in Data[title])
                    {
                        infoBuilder.Append($"|{PadCenter(value, columnTitleWidth)}");
                    }
                    infoBuilder.AppendLine();
                }
            }

            // Close the code block.
            infoBuilder.AppendLine("```");

            Queries++;

            return infoBuilder.ToString();
        }

        public string GetMonsterNames()
        {
            return "```\n" + string.Join("\n", MONSTER_LIST) + "```";
        }

        /// <summary>
        /// Deserialize json data into a dictionary.
        /// </summary>
        /// <param name="key">The key to get the values under.</param>
        /// <param name="filename">The filename to open.</param>
        /// <returns>Dictionary(string, string[])</returns>
        private Dictionary<string, string[]> GetDictionaryFromJson(string key, string filename)
        {
            // Parse the json data in a file into a JObject.
            JObject parsed = JObject.Parse(File.ReadAllText(string.Join(Path.DirectorySeparatorChar.ToString(), WycademyConst.DATA_LOCATION, "monster", $"{filename.ToLower()}.json"), Encoding.UTF8));

            // Get the requested subsection and cast it to an IDictionary.
            var rawData = parsed[key] as IDictionary<string, JToken>;
            // Convert it to a Dictionary<string, string[]> and return.
            return rawData.ToDictionary(pair => pair.Key, pair => ((string)pair.Value).Split(' '));
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
    }
}
