using HtmlAgilityPack;
using KiranicoScraper.Database;
using KiranicoScraper.Scrapers.Lists;
using System.Linq;
using System.Text.RegularExpressions;
using Wycademy.Core.Enums;

namespace KiranicoScraper.Scrapers
{
    class MonsterScraperGen : Scraper
    {
        private const string BASE_URL = "http://mhgen.kiranico.com/monster";

        /// <summary>
        /// Executes the scraper, adding Gen monsters to the database.
        /// </summary>
        public override void Execute()
        {
            foreach (var monster in ScraperListCollection.Generations.Monsters)
            {
                using (WebResponse response = GetPage(monster))
                {
                    HtmlDocument page = GetHtml(response, out DbMonsterBuilder builder);

                    builder.InitialiseMonster(monster);
                    AddHitzones(page, builder);
                    AddStagger(page, builder);
                    AddStatus(page, builder);
                    AddItemEffects(page, builder);
                    //try
                    //{
                    //    builder.Commit();
                    //}
                    //catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    //{
                    //    var ie = e.InnerException as Npgsql.PostgresException;
                    //    System.IO.File.WriteAllText($".\\{monster}.txt", $"{ie.ConstraintName}\n{ie.Detail}");
                    //}
                    builder.Commit();
                }
            }

            HandleSpecialMonsters();
        }

        /// <summary>
        /// Gets the MHGen Kiranico webpage for a monster.
        /// </summary>
        /// <param name="monster">The monster's Kiranico URL name.</param>
        private WebResponse GetPage(string monster) => Requester.GetPage($"{BASE_URL}/{monster}");

        /// <summary>
        /// Gets the HTML from a <see cref="WebResponse"/> as a HAP <see cref="HtmlDocument"/> and sets a <see cref="DbMonsterBuilder"/>.
        /// </summary>
        /// <param name="response">The <see cref="WebResponse"/> to get the HTML from.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add the monster's data.</param>
        private HtmlDocument GetHtml(WebResponse response, out DbMonsterBuilder builder)
        {
            builder = response.CreateMonsterBuilder();
            return response.GetPageAsHtml();
        }

        /// <summary>
        /// Adds a monster's hitzone data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add monster data to the database.</param>
        private void AddHitzones(HtmlDocument page, DbMonsterBuilder builder)
        {
            // The hitzone data is all contained within a div with the class col-lg-12.
            HtmlNode hitzoneDiv = page.DocumentNode.SelectSingleNode("//div[@class='col-lg-12']");

            // The div contains an ul element with a number of li elements corresponding to the number of hitzone tabs.
            var tabCount = hitzoneDiv.SelectNodes("./ul/li").Count;
            for (int i = 0; i < tabCount; i++)
            {
                // From the hitzone div, select the correct tab (given by the id attribute) and get the rows of the table body, each of which is a hitzone.
                HtmlNodeCollection tableRows = hitzoneDiv.SelectNodes($"./div/div[@id='state-{i}']/div/table/tbody/tr");

                foreach (HtmlNode row in tableRows)
                {
                    HtmlNodeCollection rowItems = row.SelectNodes("./td");

                    string hitzoneName;
                    // This will not be empty if the hitzone has a modifier (ex. Enraged).
                    if (rowItems[1].InnerText != string.Empty)
                    {
                        hitzoneName = $"{rowItems[0].InnerText} ({rowItems[1].InnerText})";
                    }
                    else
                    {
                        hitzoneName = rowItems[0].InnerText;
                    }

                    // Skip indices 0 and 1 (name and modifier) then take indices 2-9 and convert them to ints. 
                    var values = rowItems.Skip(2).Take(8).Select(n => int.Parse(n.InnerText)).ToList();

                    builder.AddHitzone(Game.Generations, hitzoneName, values);
                }
            }
        }

        /// <summary>
        /// Adds a monster's stagger/sever/extract colour data.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add monster data to the database.</param>
        private void AddStagger(HtmlDocument page, DbMonsterBuilder builder)
        {
            // Stagger/sever data is in a div with a class of col-lg-5. Get only the body rows of the table, we don't need the header.
            HtmlNodeCollection staggerRows = page.DocumentNode.SelectNodes("//div[@class='col-lg-5']/div/table/tbody/tr");
            foreach (HtmlNode row in staggerRows)
            {
                HtmlNodeCollection rowItems = row.SelectNodes("./td");

                string name = rowItems[0].InnerText;
                var stagger = int.Parse(rowItems[1].InnerText);

                string td3 = rowItems[2].InnerText;
                // If it's empty, pass null to the database, otherwise the integer value.
                int? sever = td3 == string.Empty ? null as int? : int.Parse(td3);

                string extractColour = ScraperListCollection.Generations.ExtractColours[rowItems[3].InnerText];

                builder.AddStaggerGen(name, stagger, extractColour, sever);
            }
        }

        /// <summary>
        /// Adds a monster's status susceptibility.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add monster data to the database.</param>
        private void AddStatus(HtmlDocument page, DbMonsterBuilder builder)
        {
            // Both status and item effects are in the same div and the next sibling is empty so we find the label header then move two siblings down.
            HtmlNode statusDiv = page.DocumentNode.SelectSingleNode("//div[@class='col-lg-7']/h5[text()='Abnormal Status']").NextSibling.NextSibling;

            HtmlNodeCollection statusRows = statusDiv.SelectNodes("./table/tbody/tr");
            foreach (HtmlNode row in statusRows)
            {
                HtmlNodeCollection rowItems = row.SelectNodes("./td");

                string statusName = rowItems[0].InnerText;
                
                // Get indices 1-3 which correspond to initial threshold/increase per proc/max threshold
                var values = rowItems.Skip(1).Take(3).Select(n => int.Parse(n.InnerText)).ToList();

                var durationMatch = Regex.Match(rowItems[4].InnerText, @"^(\d{1,3})sec");
                values.Add(int.Parse(durationMatch.Groups[1].Value));

                // Damage comes before reduction in the database but after reduction on Kiranico.
                // This regex matches overall damage (1-3 digits) and damage rate (damage, 2 digits/time interval, 1 digit with 1 decimal point) and compensates for varying whitespace.
                // We don't currently use the damage rate but that may change in the future so this regex captures it anyways.
                var damageMatch = Regex.Match(rowItems[6].InnerText, @" +(?<total>\d{1,3})? *(?<rate>\(\d{1,2}\/\d\.?\d?sec\))? +");
                string value = damageMatch.Groups["total"].Value;
                // If the damage is an empty string (not all status effects do damage), just pass zero.
                values.Add(value == string.Empty ? 0 : int.Parse(value));

                // Group 1 is reduction amount, group 2 is reduction rate.
                var reductionMatch = Regex.Match(rowItems[5].InnerText, @"^(\d{1,2})\/(\d{1,2})sec");
                values.Add(int.Parse(reductionMatch.Groups[2].Value));
                values.Add(int.Parse(reductionMatch.Groups[1].Value));

                builder.AddStatus(Game.Generations, statusName, values);
            }
        }

        /// <summary>
        /// Adds a monster's item susceptiblity.
        /// </summary>
        /// <param name="page">The monster's Kiranico web page.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add monster data to the database.</param>
        private void AddItemEffects(HtmlDocument page, DbMonsterBuilder builder)
        {
            // Get the item div in the same method as the status div.
            HtmlNode itemDiv = page.DocumentNode.SelectSingleNode("//div[@class='col-lg-7']/h5[text()='Item Effects']").NextSibling.NextSibling;

            HtmlNodeCollection itemRows = itemDiv.SelectNodes("./table/tbody/tr");
            foreach (HtmlNode row in itemRows)
            {
                HtmlNodeCollection rowItems = row.SelectNodes("./td");

                string itemName = rowItems[0].InnerText;

                // Get normal/enraged/exhausted item durations.
                var values = rowItems.Skip(1).Take(3).Select(n =>
                {
                    var match = Regex.Match(n.InnerText, @"^(\d{1,2})sec");
                    return int.Parse(match.Groups[1].Value);
                }).ToList();

                builder.AddItemEffect(Game.Generations, itemName, values);
            }
        }

        /// <summary>
        /// Manually process certain monsters that have an inconsistency that violates the database's unique constraints.
        /// </summary>
        private void HandleSpecialMonsters()
        {
            /* The following monsters have an inconsistency or error that makes it so that they must be processed separately:
             * 
             * tetsucabra, drilltusk-tetsucabra, royal-ludroth, shogun-ceanataur, nargacuga, silverwind-nargacuga
             */

            #region Tetsucabra
            // Reason: Unknown duplicate tail stagger zone
            using (WebResponse tetsuResponse = GetPage("tetsucabra"))
            {
                HtmlDocument tetsuHtml = GetHtml(tetsuResponse, out DbMonsterBuilder builder);

                GetStaggerZonesByNameAndValue(tetsuHtml, "Tail", 15).Single().Remove();

                builder.InitialiseMonster("tetsucabra");
                AddHitzones(tetsuHtml, builder);
                AddStagger(tetsuHtml, builder);
                AddStatus(tetsuHtml, builder);
                AddItemEffects(tetsuHtml, builder);
                builder.Commit();
            }
            #endregion

            #region Drilltusk Tetsucabra
            // Reason: Unknown duplicate tail stagger zone
            using (WebResponse drillResponse = GetPage("drilltusk-tetsucabra"))
            {
                HtmlDocument drillHtml = GetHtml(drillResponse, out DbMonsterBuilder builder);

                GetStaggerZonesByNameAndValue(drillHtml, "Tail", 100).Single().Remove();

                builder.InitialiseMonster("drilltusk-tetsucabra");
                AddHitzones(drillHtml, builder);
                AddStagger(drillHtml, builder);
                AddStatus(drillHtml, builder);
                AddItemEffects(drillHtml, builder);
                builder.Commit();
            }
            #endregion

            #region Royal Ludroth
            // Reason: 2 nameless stagger zones with the same value and extract colour
            using (WebResponse ludrothResponse = GetPage("royal-ludroth"))
            {
                HtmlDocument ludrothHtml = GetHtml(ludrothResponse, out DbMonsterBuilder builder);

                foreach(HtmlNode node in GetStaggerZonesByNameAndValue(ludrothHtml, string.Empty, 100))
                {
                    node.Remove();
                }

                builder.InitialiseMonster("royal-ludroth");
                AddHitzones(ludrothHtml, builder);
                AddStagger(ludrothHtml, builder);
                AddStatus(ludrothHtml, builder);
                AddItemEffects(ludrothHtml, builder);
                builder.Commit();
            }
            #endregion

            #region Shogun Ceanataur
            // Reason: Broken shell has different hitzones for Gravios skull shell vs normal shell
            using (WebResponse shogunResponse = GetPage("shogun-ceanataur"))
            {
                HtmlDocument shogunHtml = GetHtml(shogunResponse, out DbMonsterBuilder builder);

                // Table B has one row, we want to edit the condition that causes the alternate hitzone. XPath uses 1-based indexing.
                shogunHtml.DocumentNode.SelectSingleNode("//div[@class='col-lg-12']/div/div[@id='state-1']/div/table/tbody/tr/td[2]").InnerHtml = "(Wounded, Gravios Skull)";

                builder.InitialiseMonster("shogun-ceanataur");
                AddHitzones(shogunHtml, builder);
                AddStagger(shogunHtml, builder);
                AddStatus(shogunHtml, builder);
                AddItemEffects(shogunHtml, builder);
                builder.Commit();
            }
            #endregion

            #region Nargacuga
            // Reason: Wing Blade (Left) stagger zone is duplicated
            using (WebResponse nargaResponse = GetPage("nargacuga"))
            {
                HtmlDocument nargaHtml = GetHtml(nargaResponse, out DbMonsterBuilder builder);

                // Get both stagger zones and remove the second (index 1).
                GetStaggerZonesByNameAndValue(nargaHtml, "Wing Blade (Left)", 80).ElementAt(1).Remove();

                builder.InitialiseMonster("nargacuga");
                AddHitzones(nargaHtml, builder);
                AddStagger(nargaHtml, builder);
                AddStatus(nargaHtml, builder);
                AddItemEffects(nargaHtml, builder);
                builder.Commit();
            }
            #endregion

            #region Silverwind Nargacuga
            // Reason: Wing Blade (Left) stagger zone is duplicated
            using (WebResponse silverResponse = GetPage("silverwind-nargacuga"))
            {
                HtmlDocument silverHtml = GetHtml(silverResponse, out DbMonsterBuilder builder);

                // Get both stagger zones and remove the second (index 1).
                GetStaggerZonesByNameAndValue(silverHtml, "Wing Blade (Left)", 80).ElementAt(1).Remove();

                builder.InitialiseMonster("silverwind-nargacuga");
                AddHitzones(silverHtml, builder);
                AddStagger(silverHtml, builder);
                AddStatus(silverHtml, builder);
                AddItemEffects(silverHtml, builder);
                builder.Commit();
            }
            #endregion
        }

        /// <summary>
        /// Gets stagger nodes matching a supplied name and value.
        /// </summary>
        /// <param name="html">The <see cref="HtmlDocument"/> to search.</param>
        /// <param name="name">The name of the stagger zone.</param>
        /// <param name="value">The value of the stagger zone.</param>
        /// <returns>All the nodes matching the input.</returns>
        private HtmlNodeCollection GetStaggerZonesByNameAndValue(HtmlDocument html, string name, int value)
        {
            // Set the text predicate expression to check for either lack of text or a specific string.
            var textPredicate = string.IsNullOrEmpty(name) ? "not(text())" : $"text()='{name}'";

            // Retrieve all the <tr> elements that contain <td> elements matching both the text predicate and the stagger value.
            return html.DocumentNode.SelectNodes($"//div[@class='col-lg-5']/div/table/tbody/tr[td[{textPredicate}]][td[text()='{value}']]");
        }
    }
}
