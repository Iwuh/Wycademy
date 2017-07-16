using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Models;

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
        private readonly string[] ITEMEFFECTS_COLUMN_NAMES = new string[] { "Normal", "Enraged", "Fatigued" };

        public async Task<Dictionary<string, (string, int?)>> GetMonsterInfo(MonsterDataCategory category, string name, MonsterContext context, Expression<Func<Monster, IEnumerable<IMonsterData>>> include)
        {
            // Get the monster matching the input name, joined with its hitzones/status/items/stagger data using the supplied function.
            var monster = await context.Monsters
                            .Include(include)
                            .SingleAsync(m => m.WebName == name);

            // Get the data depending on the category requested.
            string[] columnTitles;
            IEnumerable<IMonsterData> data;
            switch (category)
            {
                case MonsterDataCategory.Hitzone:
                    data = monster.Hitzones;
                    columnTitles = HITZONE_COLUMN_NAMES;
                    break;
                case MonsterDataCategory.Status:
                    data = monster.Status;
                    columnTitles = STATUS_COLUMN_NAMES;
                    break;
                case MonsterDataCategory.Item:
                    data = monster.Items;
                    columnTitles = ITEMEFFECTS_COLUMN_NAMES;
                    break;
                case MonsterDataCategory.Stagger:
                    data = monster.Stagger;
                    columnTitles = STAGGER_COLUMN_NAMES;
                    break;
                default:
                    throw new ArgumentException($"Invalid Category: {category}");
            }

            // Find how many different games are included in the result set.
            var games = data.Select(d => d.Game).Distinct();
            Dictionary<string, (string, int?)> pages;
            if (games.Count() == 1)
            {
                pages = new Dictionary<string, (string, int?)>() { { games.First(), BuildTable(data, columnTitles) } };
            }
            else
            {
                var grouped = data.GroupBy(d => d.Game);
                pages = grouped.ToDictionary(g => g.Key, g => BuildTable(g.ToList(), columnTitles));
            }

            return pages;

            (string, int?) BuildTable(IEnumerable<IMonsterData> rows, string[] titles)
            {
                int columnTitleWidth = titles.Max(t => t.Length);
                int rowTitleWidth = rows.Max(d => d.Name.Length);

                // Create the string builder with the table's title, and open the code block.
                var tableBuilder = new StringBuilder($"{rows.First().Game} {category} info for {monster.TitleName}:");
                tableBuilder.AppendLine("```");
                // Put blank spaces in the upper left corner.
                tableBuilder.Append(' ', rowTitleWidth);
                // Add all the column titles.
                foreach (var title in titles)
                {
                    tableBuilder.Append("|" + PadCenter(title, columnTitleWidth));
                }
                tableBuilder.AppendLine();

                int? splitPoint = null;
                bool split = false;

                foreach (var row in rows)
                {
                    var rowBuilder = new StringBuilder();

                    // Add the name, plus blank spaces to keep the columns aligned.
                    rowBuilder.Append(row.Name.PadRight(rowTitleWidth));

                    // Add all the values.
                    foreach (var value in row.Values)
                    {
                        rowBuilder.Append("|" + PadCenter(value ?? "N/A", columnTitleWidth));
                    }

                    // If adding the row will bring the character count over 1990 characters (limit is 2000), mark the point where the table should be split into two messages.
                    if (tableBuilder.Length + rowBuilder.Length >= 1990 && !split)
                    {
                        // Close the first code block.
                        tableBuilder.AppendLine("```");
                        // Get the index of the split.
                        splitPoint = tableBuilder.Length - 1;
                        split = true;
                        // Open the second code block.
                        tableBuilder.AppendLine("```");
                    }
                    tableBuilder.AppendLine(rowBuilder.ToString());
                }

                tableBuilder.AppendLine("```");

                return (tableBuilder.ToString(), splitPoint);
            }

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
