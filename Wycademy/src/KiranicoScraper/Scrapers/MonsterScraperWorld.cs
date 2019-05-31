using HtmlAgilityPack;
using KiranicoScraper.Database;
using KiranicoScraper.Scrapers.Lists;
using System.Linq;
using System.Text.RegularExpressions;
using Wycademy.Core.Enums;

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
            foreach (var monster in ScraperListCollection.World.Monsters)
            {
                using (WebResponse response = Requester.GetPage($"{BASE_URL}/{monster}"))
                {
                    HtmlDocument page = response.GetPageAsHtml();
                    var builder = response.CreateMonsterBuilder();

                    // Handle Vaal Hazak here because he's the only monster with an exception.
                    if (monster == "vaal-hazak")
                    {
                        // Change the first "Tail Tip" hitzone to "Tail".
                        page.DocumentNode.SelectSingleNode("//div[@class='col-sm-8']/table/tr[td[text()='Tail Tip']]/td[1]").InnerHtml = "Tail";
                    }

                    builder.InitialiseMonster(monster);
                    AddHitzones(page, builder);
                    AddStagger(page, builder);
                    builder.Commit();
                }
            }
        }

        /// <summary>
        /// Adds a monster's hitzone data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add data about the monster to the database.</param>
        private void AddHitzones(HtmlDocument page, DbMonsterBuilder builder)
        {
            HtmlNodeCollection hitzoneRows = page.DocumentNode.SelectNodes("//div[@class='col-sm-8']/table/tr");
            foreach (HtmlNode row in hitzoneRows)
            {
                HtmlNodeCollection rowItems = row.SelectNodes("./td");

                // The name column includes any modifiers if applicable.
                string name = rowItems[0].InnerText;

                // Get indices 1-8, ignore 9 because we don't need stun data.
                var values = rowItems.Skip(1).Take(8).Select(n => int.Parse(n.InnerText)).ToList();
                // MHW Kiranico has thunder before ice but we need to switch that around for the database.
                values.Swap(6, 7);

                builder.AddHitzone(Game.World, name, values);
            }
        }

        /// <summary>
        /// Adds a monster's stagger data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add data about the monster to the database.</param>
        private void AddStagger(HtmlDocument page, DbMonsterBuilder builder)
        {
            HtmlNodeCollection staggerRows = page.DocumentNode.SelectNodes("//div[@class='col-sm-4']/table/tr");
            foreach (HtmlNode row in staggerRows)
            {
                HtmlNodeCollection rowItems = row.SelectNodes("./td");

                string name = rowItems[0].InnerText.Trim();
                // The attribute corresponding to the extract colour is the class of the i element that is a direct child of the first td.
                string extract = ScraperListCollection.World.BugExtracts[rowItems[0].SelectSingleNode("./i").Attributes["class"].Value];

                var stagger = int.Parse(rowItems[1].InnerText);

                // The only reason we even use this regex is because Kirin has an asterisk after its break value, which prevents us from simply parsing the raw text.
                var woundMatch = Regex.Match(rowItems[2].InnerText, @"(\d+)?");
                var group = woundMatch.Groups[1].Value;
                int? wound = group == string.Empty ? null as int? : int.Parse(group);

                // Sever doesn't suffer from the same issue so we can just parse the string content if applicable.
                var severString = rowItems[3].InnerText;
                int? sever = severString == string.Empty ? null as int? : int.Parse(severString);

                builder.AddStaggerWorld(name, stagger, extract, sever, wound);
            }
        }
    }
}
