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
        public int Queries { get; private set; }

        public async Task<string> GetMonsterInfo(string category, string monsterName)
        {
            // Deserialise the json on a threadpool to avoid blocking while reading from disk.
            Dictionary<string, string[]> Data = await Task.Run(() => GetDictionaryFromJson(category, monsterName));

            StringBuilder infoBuilder = new StringBuilder();
            int columnTitleWidth;
            int rowTitleWidth;
            string[] columnNames;

            // Get the appropriate column titles depending on the category.
            switch (category)
            {
                case "Hitzone":
                    columnNames = WycademyConst.HITZONE_COLUMN_NAMES;
                    break;
                case "Status":
                    columnNames = WycademyConst.STATUS_COLUMN_NAMES;
                    break;
                case "Stagger/Sever":
                    columnNames = WycademyConst.STAGGER_COLUMN_NAMES;
                    break;
                case "Item Effects":
                    columnNames = WycademyConst.ITEMEFFECTS_COLUMN_NAMES;
                    break;
                default:
                    throw new ArgumentException($"{category} is not a valid category.");
            }
            // Set the widths of the row title column and the data columns.
            columnTitleWidth = columnNames.Max(x => x.Length);
            rowTitleWidth = Data.Keys.Max(x => x.Length);

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

        private Dictionary<string, string[]> GetDictionaryFromJson(string key, string filename)
        {
            // Parse the json data in a file into a JObject.
            JObject parsed = JObject.Parse(File.ReadAllText($"{WycademyConst.DATA_FOLDER}\\{filename}.json"));

            // Get the requested subsection and cast it to an IDictionary.
            var rawData = parsed[key] as IDictionary<string, JToken>;
            // Convert it to a Dictionary<string, string[]> and return.
            return rawData.ToDictionary(pair => pair.Key, pair => ((string)pair.Value).Split(' '));
        }

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
