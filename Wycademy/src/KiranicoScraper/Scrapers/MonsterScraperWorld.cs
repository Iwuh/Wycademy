using HtmlAgilityPack;
using KiranicoScraper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KiranicoScraper.Scrapers
{
    class MonsterScraperWorld : Scraper
    {
        private const string BASE_URL = "https://mhworld.kiranico.com/monster";

        /// <summary>
        /// Executes the scraper, adding World monsters to the database.
        /// </summary>
        public override void Execute()
        {
            foreach (var monster in Database.ScraperLists.World.Monsters)
            {
                int monsterId = Database.AddMonsterAndGetId(monster);

                var page = Requester.GetHtml($"{BASE_URL}/{monster}");

                AddHitzones(page, monsterId);
                AddStagger(page, monsterId);
            }
        }

        /// <summary>
        /// Adds a monster's hitzone data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddHitzones(HtmlDocument page, int monsterId)
        {
            var hitzoneDiv = page.DocumentNode.SelectSingleNode("//div[@class='col-sm-8']");

            var hitzoneRows = hitzoneDiv.SelectNodes("./table/tr");
            foreach (var row in hitzoneRows)
            {
                var rowItems = row.SelectNodes("./td");

                // The name column includes any modifiers if applicable.
                var name = rowItems[0].InnerText;

                // Get indices 1-8, ignore 9 because we don't need stun data.
                var values = rowItems.Skip(1).Take(8).Select(n => int.Parse(n.InnerText)).ToList();
                // MHW Kiranico has thunder before ice but we need to switch that around for the database.
                values.Swap(6, 7);

                Database.AddHitzone(monsterId, Game.World, name, values);
            }
        }

        /// <summary>
        /// Adds a monster's stagger data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddStagger(HtmlDocument page, int monsterId)
        {
            var staggerDiv = page.DocumentNode.SelectSingleNode("//div[@class='col-sm-4']");

            var staggerRows = staggerDiv.SelectNodes("./table/tr");
            foreach (var row in staggerRows)
            {
                var rowItems = row.SelectNodes("./td");

                var name = rowItems[0].InnerText.Trim();
                // The attribute corresponding to the extract colour is the class of the i element that is a direct child of the first td.
                var extract = Database.ScraperLists.World.BugExtracts[rowItems[0].SelectSingleNode("./i").Attributes["class"].Value];

                var stagger = int.Parse(rowItems[1].InnerText);

                // The only reason we even use this regex is because Kirin has an asterisk after its break value, which prevents us from simply parsing the raw text.
                var woundMatch = Regex.Match(rowItems[2].InnerText, @"(\d+)?");
                var group = woundMatch.Groups[1].Value;
                int? wound = group == string.Empty ? null as int? : int.Parse(group);

                // Sever doesn't suffer from the same issue so we can just parse the string content if applicable.
                var severString = rowItems[3].InnerText;
                int? sever = severString == string.Empty ? null as int? : int.Parse(severString);

                Database.AddStaggerWorld(monsterId, name, stagger, extract, sever, wound);
            }
        }
    }
}
