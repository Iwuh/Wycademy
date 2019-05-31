using KiranicoScraper.Database;
using KiranicoScraper.Scrapers.Lists;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Wycademy.Core.Enums;

namespace KiranicoScraper.Scrapers
{
    class MonsterScraper4U : Scraper
    {
        private const string BASE_URL = "http://kiranico.com/en/mh4u/monster";

        /// <summary>
        /// Executes this scraper, adding data for 4U monsters to the database.
        /// </summary>
        public override void Execute()
        {
            foreach (var monster in ScraperListCollection.FourUltimate.Monsters)
            {
                using (WebResponse page = GetPage(monster))
                {
                    JObject json = GetJson(page, out DbMonsterBuilder builder);

                    builder.InitialiseMonster(monster);
                    AddHitzones(json, monster, builder);
                    AddStagger(json, builder);
                    AddStatus(json, builder);
                    AddItemEffects(json, builder);
                    builder.Commit();
                }
            }

            // Some monsters have inconsistensies that make them unable to be automatically processed, so we do it manually.
            HandleSpecialMonsters();
        }

        /// <summary>
        /// Gets the json data for a monster from the response page.
        /// </summary>
        /// <param name="response">The response to get the json from.</param>
        /// <param name="builder">A <see cref="DbMonsterBuilder"/> used to add monster data to the database.</param>
        private JObject GetJson(WebResponse response, out DbMonsterBuilder builder)
        {
            builder = response.CreateMonsterBuilder();
            return (JObject)response.GetPageAsJson("{\"monster\":", "]}}")["monster"];
        }

        /// <summary>
        /// Gets the page for a monster.
        /// </summary>
        /// <param name="monster">The monster's url name.</param>
        private WebResponse GetPage(string monster) => Requester.GetPage($"{BASE_URL}/{monster}");

        /// <summary>
        /// Adds hitzones for a monster with up to one table of alternate hitzones.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monster">The monster's name.</param>
        /// <param name="builder">The <see cref="DbMonsterBuilder"/> for this monster.</param>
        private void AddHitzones(JObject json, string monster, DbMonsterBuilder builder)
        {
            var alternateNames = ScraperListCollection.FourUltimate.AltHitzones;

            foreach (var hitzone in json["monsterbodyparts"])
            {
                var hitzonePivot = hitzone["pivot"];

                var localName = (string)hitzone["local_name"];
                // If the type is A (normal conditions) use the part name alone, otherwise append the condition that causes the hitzone to change.
                var hitzoneName = (string)hitzonePivot["type"] == "A" ? localName : $"{localName} ({alternateNames[monster]})";

                // Get the hitzone value for each damage type.
                var hitzoneValues = ScraperListCollection.FourUltimate.HitzoneKeys.Select(k => (int)hitzonePivot[k]).ToList();

                builder.AddHitzone(Game.Four, hitzoneName, hitzoneValues);
            }
        }

        /// <summary>
        /// Adds hitzones for a monster with three tables (two tables of alternate hitzones).
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monster">The monster's name.</param>
        /// <param name="builder">The <see cref="DbMonsterBuilder"/> for this monster.</param>
        private void AddHitzonesWithThreeTables(JObject json, string monster, DbMonsterBuilder builder)
        {
            var altNames = ScraperListCollection.FourUltimate.DoubleAltHitzones;

            foreach (var hitzone in json["monsterbodyparts"])
            {
                var hitzonePivot = hitzone["pivot"];

                var localName = (string)hitzone["local_name"];
                string hitzoneName = null;
                // A: default; B: first alternate condition; C: second alternate condition
                switch ((string)hitzonePivot["type"])
                {
                    case "A":
                        hitzoneName = localName;
                        break;
                    case "B":
                        hitzoneName = $"{localName} ({altNames[monster][0]})";
                        break;
                    case "C":
                        hitzoneName = $"{localName} ({altNames[monster][1]})";
                        break;
                }

                var hitzoneValues = ScraperListCollection.FourUltimate.HitzoneKeys.Select(k => (int)hitzonePivot[k]).ToList();

                builder.AddHitzone(Game.Four, hitzoneName, hitzoneValues);
            }
        }

        /// <summary>
        /// Adds stagger data for a monster.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="builder">The <see cref="DbMonsterBuilder"/> for this monster.</param>
        private void AddStagger(JObject json, DbMonsterBuilder builder)
        {
            foreach (var stagger in json["monsterstaggerlimits"])
            {
                // Some table rows just have an empty name. I don't know why, they don't seem to be relevant, so we just skip them.
                var region = (string)stagger["region"];
                if (region == string.Empty) continue;

                var value = (int)stagger["value"];
                var colour = Utils.TitleCase((string)stagger["extract_color"]);

                builder.AddStagger4U(region, value, colour);
            }
        }

        /// <summary>
        /// Adds status data for a monster.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="builder">The <see cref="DbMonsterBuilder"/> for this monster.</param>
        private void AddStatus(JToken json, DbMonsterBuilder builder)
        {
            foreach (var status in json["weaponspecialattacks"])
            {
                var statusPivot = status["pivot"];

                var name = (string)status["local_name"];

                var statusValues = ScraperListCollection.FourUltimate.StatusKeys.Select(k => (int)statusPivot[k]).ToList();

                builder.AddStatus(Game.Four, name, statusValues);
            }
        }

        /// <summary>
        /// Adds a monster's item suceptibility.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="builder">The <see cref="DbMonsterBuilder"/> for this monster.</param>
        private void AddItemEffects(JToken json, DbMonsterBuilder builder)
        {
            foreach (var item in json["itemeffects"])
            {
                var itemPivot = item["pivot"];

                var name = (string)item["local_name"];

                var values = ScraperListCollection.FourUltimate.ItemEffectKeys.Select(k => (int)itemPivot[k]).ToList();

                builder.AddItemEffect(Game.Four, name, values);
            }
        }

        /// <summary>
        /// Manually process the monsters that have some error, inconsistency, or missing data such that they can't be processed normally.
        /// Most data missing from Kiranico was retrieved from the MH4G wiki and cross-referenced with Ping's Dex for accuracy.
        /// </summary>
        private void HandleSpecialMonsters()
        {
            /* The following monsters have an inconsistency or error that makes it so that they must be processed separately:
             * 
             * cephadrome, plum-d.hermitaur, najarala, brachydios, raging-brachydios, khezu, red-khezu, gravios, azure-rathalos, gore-magala, chaotic-gore-magala, 
             * dalamadur, shah-dalamadur, gogmazios, fatalis, crimson-fatalis, white-fatalis
             */

            #region Cephadrome
            // Reason: No hitzones

            using (WebResponse cephaResponse = GetPage("cephadrome"))
            {
                JObject cephaJson = GetJson(cephaResponse, out DbMonsterBuilder cephaBuilder);
                cephaBuilder.InitialiseMonster("cephadrome");

                var cephaHitzones = new Dictionary<string, int[]>
                {
                    { "Head",          new[] { 35, 40, 50, 0, 10, 15, 30, 5 } },
                    { "Neck",          new[] { 65, 60, 80, 5, 10, 15, 15, 5 } },
                    { "Belly",         new[] { 50, 55, 50, 0, 15, 15, 20, 5 } },
                    { "Back / Fin",    new[] { 60, 50, 80, 0, 12, 20, 17, 5 } },
                    { "Wing Membrane", new[] { 42, 35, 50, 0, 10, 10, 15, 5 } },
                    { "Legs",          new[] { 35, 40, 40, 0, 10, 10, 15, 5 } },
                    { "Tail",          new[] { 30, 30, 30, 0, 15, 10, 20, 5 } }
                };

                cephaHitzones.ForEach(p => cephaBuilder.AddHitzone(Game.Four, p.Key, p.Value));
                AddStagger(cephaJson, cephaBuilder);
                AddStatus(cephaJson, cephaBuilder);
                AddItemEffects(cephaJson, cephaBuilder);
                cephaBuilder.Commit();
            }

            #endregion

            #region Plum Daimyo Hermitaur
            // Reason: Shell / Claws (Broken) should not include the (Guarding) modifier

            using (WebResponse daimyoResponse = GetPage("plum-d.hermitaur"))
            {
                JObject daimyoJson = GetJson(daimyoResponse, out DbMonsterBuilder daimyoBuilder);
                daimyoBuilder.InitialiseMonster("plum-d.hermitaur");

                // Remove the offending hitzone from the JSON.
                daimyoJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Shell / Claws (Broken)").Remove();
                // Add the rest of the hitzones then manually add the last hitzone.
                AddHitzones(daimyoJson, "plum-d.hermitaur", daimyoBuilder);
                daimyoBuilder.AddHitzone(Game.Four, "Shell / Claws (Broken)", new[] { 30, 40, 25, 10, 5, 15, 10, 0 });

                AddStagger(daimyoJson, daimyoBuilder);
                AddStatus(daimyoJson, daimyoBuilder);
                AddItemEffects(daimyoJson, daimyoBuilder);
                daimyoBuilder.Commit();
            }
            #endregion

            #region Najarala
            // Reason: Tail hitzone is duplicated in the second table

            using (WebResponse najaResponse = GetPage("najarala"))
            {
                JObject najaJson = GetJson(najaResponse, out DbMonsterBuilder najaBuilder);
                najaBuilder.InitialiseMonster("najarala");

                najaJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Tail" && (string)t["pivot"]["type"] == "B").Remove();

                AddHitzones(najaJson, "najarala", najaBuilder);
                AddStagger(najaJson, najaBuilder);
                AddStatus(najaJson, najaBuilder);
                AddItemEffects(najaJson, najaBuilder);
                najaBuilder.Commit();
            }
            #endregion

            #region Brachydios & Raging Brachydios
            // Reason: Raging Brachydios hitzones are on Brachydios' page

            using (WebResponse brachResponse = GetPage("brachydios"))
            using (WebResponse ragingBrachResponse = GetPage("raging-brachydios"))
            {
                JObject brachJson = GetJson(brachResponse, out DbMonsterBuilder brachBuilder);
                var brachJsonClone = (JObject)brachJson.DeepClone();

                brachBuilder.InitialiseMonster("brachydios");

                ((JArray)brachJson["monsterbodyparts"]).RemoveRange(7);
                AddHitzones(brachJson, "brachydios", brachBuilder);

                AddStagger(brachJson, brachBuilder);
                AddStatus(brachJson, brachBuilder);
                AddItemEffects(brachJson, brachBuilder);
                brachBuilder.Commit();

                JObject ragingBrachJson = GetJson(ragingBrachResponse, out DbMonsterBuilder ragingBrachBuilder);
                ragingBrachBuilder.InitialiseMonster("raging-brachydios");

                // Use the clone of normal Brachydios json for hitzones, Raging Brachydios json for everything else.
                ((JArray)brachJsonClone["monsterbodyparts"]).RemoveRange(0, 7);
                AddHitzones(brachJsonClone, "raging-brachydios", ragingBrachBuilder);

                AddStagger(ragingBrachJson, ragingBrachBuilder);
                AddStatus(ragingBrachJson, ragingBrachBuilder);
                AddItemEffects(ragingBrachJson, ragingBrachBuilder);
                ragingBrachBuilder.Commit();
            }
            #endregion

            #region Khezu
            // Reason: First 'Head' stagger limit should be 'Body'

            using (WebResponse khezuResponse = GetPage("khezu"))
            {
                JObject khezuJson = GetJson(khezuResponse, out DbMonsterBuilder khezuBuilder);

                JToken error = khezuJson["monsterstaggerlimits"].First(t => (string)t["region"] == "Head" && (string)t["extract_color"] == "orange");
                error["region"] = "Body";

                khezuBuilder.InitialiseMonster("khezu");
                AddHitzones(khezuJson, "khezu", khezuBuilder);
                AddStatus(khezuJson, khezuBuilder);
                AddStagger(khezuJson, khezuBuilder);
                AddItemEffects(khezuJson, khezuBuilder);
                khezuBuilder.Commit();
            }
            #endregion

            #region Red Khezu
            // Reason: First 'Head' stagger limit should be 'Body'

            using (WebResponse redKhezuResponse = GetPage("red-khezu"))
            {
                JObject redKhezuJson = GetJson(redKhezuResponse, out DbMonsterBuilder redKhezuBuilder);

                JToken error = redKhezuJson["monsterstaggerlimits"].First(t => (string)t["region"] == "Head" && (string)t["extract_color"] == "orange");
                error["region"] = "Body";

                redKhezuBuilder.InitialiseMonster("red-khezu");
                AddHitzones(redKhezuJson, "red-khezu", redKhezuBuilder);
                AddStatus(redKhezuJson, redKhezuBuilder);
                AddStagger(redKhezuJson, redKhezuBuilder);
                AddItemEffects(redKhezuJson, redKhezuBuilder);
                redKhezuBuilder.Commit();
            }
            #endregion

            #region Gravios
            // Reason: Underbody hitzone is duplicated in second table

            using (WebResponse gravResponse = GetPage("gravios"))
            {
                JObject gravJson = GetJson(gravResponse, out DbMonsterBuilder gravBuilder);
                gravBuilder.InitialiseMonster("gravios");

                gravJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Underbody" && (string)t["pivot"]["type"] == "B").Remove();

                AddHitzones(gravJson, "gravios", gravBuilder);
                AddStagger(gravJson, gravBuilder);
                AddStatus(gravJson, gravBuilder);
                AddItemEffects(gravJson, gravBuilder);
                gravBuilder.Commit();
            }
            #endregion

            #region Azure Rathalos
            // Reason: Azure Rathalos' page contains Pink Rathian's hitzones

            using (WebResponse azureResponse = GetPage("azure-rathalos"))
            {
                JObject azureJson = GetJson(azureResponse, out DbMonsterBuilder azureBuilder);

                ((JArray)azureJson["monsterbodyparts"]).RemoveRange(8);

                azureBuilder.InitialiseMonster("azure-rathalos");
                AddHitzones(azureJson, "azure-rathalos", azureBuilder);
                AddStatus(azureJson, azureBuilder);
                AddStagger(azureJson, azureBuilder);
                AddItemEffects(azureJson, azureBuilder);
                azureBuilder.Commit();
            }
            #endregion

            #region Gore Magala & Chaotic Gore Magala
            // Reason: Chaotic Gore Magala's hitzones are on Gore Magala's page

            using (WebResponse goreResponse = GetPage("gore-magala"))
            using(WebResponse chaosGoreResponse = GetPage("chaotic-gore-magala"))
            {
                JObject goreJson = GetJson(goreResponse, out DbMonsterBuilder goreBuilder);
                var goreJsonClone = (JObject)goreJson.DeepClone();

                goreBuilder.InitialiseMonster("gore-magala");

                ((JArray)goreJson["monsterbodyparts"]).RemoveRange(10);
                AddHitzones(goreJson, "gore-magala", goreBuilder);

                AddStagger(goreJson, goreBuilder);
                AddStatus(goreJson, goreBuilder);
                AddItemEffects(goreJson, goreBuilder);
                goreBuilder.Commit();

                JObject chaosGoreJson = GetJson(chaosGoreResponse, out DbMonsterBuilder chaosGoreBuilder);
                chaosGoreBuilder.InitialiseMonster("chaotic-gore-magala");

                // Use clone for hitzones, proper json for everything else.
                ((JArray)goreJsonClone["monsterbodyparts"]).RemoveRange(0, 10);
                AddHitzones(goreJsonClone, "chaotic-gore-magala", chaosGoreBuilder);

                AddStagger(chaosGoreJson, chaosGoreBuilder);
                AddStatus(chaosGoreJson, chaosGoreBuilder);
                AddItemEffects(chaosGoreJson, chaosGoreBuilder);
                chaosGoreBuilder.Commit();
            }
            #endregion

            #region Dalamadur
            // Reason: Kiranico is missing the hitzone table for after parts are broken

            using (WebResponse dalaResponse = GetPage("dalamadur"))
            {
                JObject dalaJson = GetJson(dalaResponse, out DbMonsterBuilder dalaBuilder);
                dalaBuilder.InitialiseMonster("dalamadur");

                var brokenDalaHitzones = new Dictionary<string, int[]>
                {
                    { "Head (Broken)",       new[] { 55, 55, 30, 0, 0, 10, 10, 15 } },
                    { "Front Legs (Broken)", new[] { 55, 55, 15, 5, 5, 10, 10, 20 } },
                    { "Back Leg (Broken)",   new[] { 17, 17, 15, 5, 5, 10, 10, 15 } },
                    { "Tail (Broken)",       new[] { 33, 33, 10, 5, 5, 10, 10, 15 } },
                    { "Tail Tip (Broken)",   new[] { 75, 75, 60, 5, 5, 10, 10, 15 } }
                };

                AddHitzones(dalaJson, "dalamadur", dalaBuilder);
                brokenDalaHitzones.ForEach(p => dalaBuilder.AddHitzone(Game.Four, p.Key, p.Value));

                AddStagger(dalaJson, dalaBuilder);
                AddStatus(dalaJson, dalaBuilder);
                AddItemEffects(dalaJson, dalaBuilder);
                dalaBuilder.Commit();
            }
            #endregion

            #region Shah Dalamadur
            // Reason: 3 hitzone tables; no stagger names

            using (WebResponse shahResponse = GetPage("shah-dalamadur"))
            {
                JObject shahJson = GetJson(shahResponse, out DbMonsterBuilder shahBuilder);
                shahBuilder.InitialiseMonster("shah-dalamadur");

                AddHitzonesWithThreeTables(shahJson, "shah-dalamadur", shahBuilder);
                AddStatus(shahJson, shahBuilder);
                AddItemEffects(shahJson, shahBuilder);
                shahBuilder.Commit();
            }
            #endregion

            #region Gogmazios
            // Reason: 3 hitzone tables

            using (WebResponse gogResponse = GetPage("gogmazios"))
            {
                JObject gogJson = GetJson(gogResponse, out DbMonsterBuilder gogBuilder);
                gogBuilder.InitialiseMonster("gogmazios");

                AddHitzonesWithThreeTables(gogJson, "gogmazios", gogBuilder);
                AddStagger(gogJson, gogBuilder);
                AddStatus(gogJson, gogBuilder);
                AddItemEffects(gogJson, gogBuilder);
                gogBuilder.Commit();
            }
            #endregion

            #region Fatalis
            // Reason: No stagger names

            using (WebResponse fataResponse = GetPage("fatalis"))
            {
                JObject fataJson = GetJson(fataResponse, out DbMonsterBuilder fataBuilder);
                fataBuilder.InitialiseMonster("fatalis");

                AddHitzones(fataJson, "fatalis", fataBuilder);
                AddStatus(fataJson, fataBuilder);
                AddItemEffects(fataJson, fataBuilder);
                fataBuilder.Commit();
            }
            #endregion

            #region Crimson Fatalis
            // Reason: No hitzones; no stagger names

            using (WebResponse crimResponse = GetPage("crimson-fatalis"))
            {
                JObject crimJson = GetJson(crimResponse, out DbMonsterBuilder crimBuilder);
                crimBuilder.InitialiseMonster("crimson-fatalis");

                var crimHitzones = new Dictionary<string, int[]>
                {
                    { "Face",         new[] { 50, 45, 45, 15, 5, 5, 5, 80 } },
                    { "Head",         new[] { 30, 25, 30, 15, 5, 5, 5, 50 } },
                    { "Neck",         new[] { 30, 25, 25, 15, 5, 5, 5, 30 } },
                    { "Chest",        new[] { 30, 15, 20, 15, 5, 5, 5, 10 } },
                    { "Wing",         new[] { 30, 25, 20, 15, 5, 5, 5, 10 } },
                    { "Back / Tail",  new[] { 10, 20, 20, 15, 5, 5, 5, 10 } },
                    { "Belly / Legs", new[] { 20, 20, 20, 15, 5, 5, 5, 20 } }
                };
                crimHitzones.ForEach(p => crimBuilder.AddHitzone(Game.Four, p.Key, p.Value));

                AddStatus(crimJson, crimBuilder);
                AddItemEffects(crimJson, crimBuilder);
                crimBuilder.Commit();
            }
            #endregion

            #region White Fatalis
            // Reason: No hitzones; no stagger names

            using (WebResponse whiteResponse = GetPage("white-fatalis"))
            {
                JObject whiteJson = GetJson(whiteResponse, out DbMonsterBuilder whiteBuilder);
                whiteBuilder.InitialiseMonster("white-fatalis");

                var whiteHitzones = new Dictionary<string, int[]>
                {
                    { "Face",                new[] { 80, 75, 45, 15, 5, 5, 10, 80 } },
                    { "Head",                new[] { 50, 55, 30, 15, 5, 5, 10, 50 } },
                    { "Neck",                new[] { 30, 25, 25, 15, 5, 5, 10, 30 } },
                    { "Chest",               new[] { 30, 15, 20, 15, 5, 5, 10, 10 } },
                    { "Wing",                new[] { 30, 25, 20, 15, 5, 5, 10, 10 } },
                    { "Back / Tail",         new[] { 10, 20, 20, 15, 5, 5, 10, 10 } },
                    { "Belly / Legs",        new[] { 20, 20, 20, 15, 5, 5, 10, 20 } },
                    { "All Parts (Enraged)", new[] { 10, 10, 10, 10, 5, 5, 10, 10 } }
                };
                whiteHitzones.ForEach(p => whiteBuilder.AddHitzone(Game.Four, p.Key, p.Value));

                AddStatus(whiteJson, whiteBuilder);
                AddItemEffects(whiteJson, whiteBuilder);
                whiteBuilder.Commit();
            }
            #endregion
        }
    }
}
