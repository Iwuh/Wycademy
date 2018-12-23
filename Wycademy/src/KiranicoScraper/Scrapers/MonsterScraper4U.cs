using KiranicoScraper.Enums;
using KiranicoScraper.Scrapers.Lists;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            foreach (var monster in Database.ScraperLists.FourUltimate.Monsters)
            {
                int monsterId = Database.AddMonsterAndGetId(monster);

                var json = GetJson(monster);

                AddHitzones(json, monster, monsterId);

                AddStagger(json, monsterId);

                AddStatus(json, monsterId);

                AddItemEffects(json, monsterId);
            }

            // Some monsters have inconsistensies that make them unable to be automatically processed, so we do it manually.
            HandleSpecialMonsters();
        }

        /// <summary>
        /// Gets the json data for a given monster.
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        private JObject GetJson(string monster)
        {
            var json = Requester.GetJson($"{BASE_URL}/{monster}", "{\"monster\":", "]}}");
            // Get the actual monster data from an outside object.
            return (JObject)json["monster"];
        }

        /// <summary>
        /// Adds hitzones for a monster with up to one table of alternate hitzones.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monster">The monster's name.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddHitzones(JToken json, string monster, int monsterId)
        {
            var alternateNames = Database.ScraperLists.FourUltimate.AltHitzones;

            foreach (var hitzone in json["monsterbodyparts"])
            {
                var hitzonePivot = hitzone["pivot"];

                var localName = (string)hitzone["local_name"];
                // If the type is A (normal conditions) use the part name alone, otherwise append the condition that causes the hitzone to change.
                var hitzoneName = (string)hitzonePivot["type"] == "A" ? localName : $"{localName} ({alternateNames[monster]})";

                // Get the hitzone value for each damage type.
                var hitzoneValues = Database.ScraperLists.FourUltimate.HitzoneKeys.Select(k => (int)hitzonePivot[k]).ToList();

                Database.AddHitzone(monsterId, Game.Four, hitzoneName, hitzoneValues);
            }
        }

        /// <summary>
        /// Adds hitzones for a monster with three tables (two tables of alternate hitzones).
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monster">The monster's name.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddHitzonesWithThreeTables(JToken json, string monster, int monsterId)
        {
            var altNames = Database.ScraperLists.FourUltimate.DoubleAltHitzones;

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

                var hitzoneValues = Database.ScraperLists.FourUltimate.HitzoneKeys.Select(k => (int)hitzonePivot[k]).ToList();

                Database.AddHitzone(monsterId, Game.Four, hitzoneName, hitzoneValues);
            }
        }

        /// <summary>
        /// Adds stagger data for a monster.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddStagger(JToken json, int monsterId)
        {
            foreach (var stagger in json["monsterstaggerlimits"])
            {
                // Some table rows just have an empty name. I don't know why, they don't seem to be relevant, so we just skip them.
                var region = (string)stagger["region"];
                if (region == string.Empty) continue;

                var value = (int)stagger["value"];
                var colour = Utils.TitleCase((string)stagger["extract_color"]);

                Database.AddStagger4U(monsterId, region, value, colour);
            }
        }

        /// <summary>
        /// Adds status data for a monster.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddStatus(JToken json, int monsterId)
        {
            foreach (var status in json["weaponspecialattacks"])
            {
                var statusPivot = status["pivot"];

                var name = (string)status["local_name"];

                var statusValues = Database.ScraperLists.FourUltimate.StatusKeys.Select(k => (int)statusPivot[k]).ToList();

                Database.AddStatus(monsterId, Game.Four, name, statusValues);
            }
        }

        /// <summary>
        /// Adds a monster's item suceptibility.
        /// </summary>
        /// <param name="json">The monster's json data.</param>
        /// <param name="monsterId">The monster's database ID.</param>
        private void AddItemEffects(JToken json, int monsterId)
        {
            foreach (var item in json["itemeffects"])
            {
                var itemPivot = item["pivot"];

                var name = (string)item["local_name"];

                var values = Database.ScraperLists.FourUltimate.ItemEffectKeys.Select(k => (int)itemPivot[k]).ToList();

                Database.AddItemEffect(monsterId, Game.Four, name, values);
            }
        }

        /// <summary>
        /// Manually process the 14 monsters that have some error, inconsistency, or missing data such that they can't be processed normally.
        /// Most data missing from Kiranico was retrieved from the MH4G wiki and cross-referenced with Ping's Dex for accuracy.
        /// </summary>
        private void HandleSpecialMonsters()
        {
            /* The following monsters have an inconsistency or error that makes it so that they must be processed separately:
             * 
             * cephadrome, plum-d.hermitaur, najarala, brachydios, raging-brachydios, gravios, gore-magala, chaotic-gore-magala, 
             * dalamadur, shah-dalamadur, gogmazios, fatalis, crimson-fatalis, white-fatalis
             */

            #region Cephadrome
            // Reason: No hitzones

            var cephaJson = GetJson("cephadrome");
            var cephaId = Database.AddMonsterAndGetId("cephadrome");

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

            cephaHitzones.ForEach(p => Database.AddHitzone(cephaId, Game.Four, p.Key, p.Value));
            AddStagger(cephaJson, cephaId);
            AddStatus(cephaJson, cephaId);
            AddItemEffects(cephaJson, cephaId);

            #endregion

            #region Plum Daimyo Hermitaur
            // Reason: Shell / Claws (Broken) should not include the (Guarding) modifier

            var daimyoJson = GetJson("plum-d.hermitaur");
            var daimyoId = Database.AddMonsterAndGetId("plum-d.hermitaur");

            // Remove the offending hitzone from the JSON.
            daimyoJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Shell / Claws (Broken)").Remove();
            // Add the rest of the hitzones then manually add the last hitzone.
            AddHitzones(daimyoJson, "plum-d.hermitaur", daimyoId);
            Database.AddHitzone(daimyoId, Game.Four, "Shell / Claws (Broken)", new[] { 30, 40, 25, 10, 5, 15, 10, 0 });

            AddStagger(daimyoJson, daimyoId);
            AddStatus(daimyoJson, daimyoId);
            AddItemEffects(daimyoJson, daimyoId);
            #endregion

            #region Najarala
            // Reason: Tail hitzone is duplicated in the second table

            var najaJson = GetJson("najarala");
            var najaId = Database.AddMonsterAndGetId("najarala");

            najaJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Tail" && (string)t["pivot"]["type"] == "B").Remove();

            AddHitzones(najaJson, "najarala", najaId);
            AddStagger(najaJson, najaId);
            AddStatus(najaJson, najaId);
            AddItemEffects(najaJson, najaId);
            #endregion

            #region Brachydios & Raging Brachydios
            // Reason: Raging Brachydios hitzones are on Brachydios' page

            var brachJson = GetJson("brachydios");
            var brachJsonClone = brachJson.DeepClone();

            var brachId = Database.AddMonsterAndGetId("brachydios");

            ((JArray)brachJson["monsterbodyparts"]).RemoveRange(7);
            AddHitzones(brachJson, "brachydios", brachId);

            AddStagger(brachJson, brachId);
            AddStatus(brachJson, brachId);
            AddItemEffects(brachJson, brachId);

            var ragingBrachJson = GetJson("raging-brachydios");
            var ragingBrachId = Database.AddMonsterAndGetId("raging-brachydios");

            // Use the clone of normal Brachydios json for hitzones, Raging Brachydios json for everything else.
            ((JArray)brachJsonClone["monsterbodyparts"]).RemoveRange(0, 7);
            AddHitzones(brachJsonClone, "raging-brachydios", ragingBrachId);

            AddStagger(ragingBrachJson, ragingBrachId);
            AddStatus(ragingBrachJson, ragingBrachId);
            AddItemEffects(ragingBrachJson, ragingBrachId);
            #endregion

            #region Gravios
            // Reason: Underbody hitzone is duplicated in second table

            var gravJson = GetJson("gravios");
            var gravId = Database.AddMonsterAndGetId("gravios");

            gravJson["monsterbodyparts"].First(t => (string)t["local_name"] == "Underbody" && (string)t["pivot"]["type"] == "B").Remove();

            AddHitzones(gravJson, "gravios", gravId);
            AddStagger(gravJson, gravId);
            AddStatus(gravJson, gravId);
            AddItemEffects(gravJson, gravId);
            #endregion

            #region Gore Magala & Chaotic Gore Magala
            // Reason: Chaotic Gore Magala's hitzones are on Gore Magala's page

            var goreJson = GetJson("gore-magala");
            var goreJsonClone = goreJson.DeepClone();

            var goreId = Database.AddMonsterAndGetId("gore-magala");

            ((JArray)goreJson["monsterbodyparts"]).RemoveRange(10);
            AddHitzones(goreJson, "gore-magala", goreId);

            AddStagger(goreJson, goreId);
            AddStatus(goreJson, goreId);
            AddItemEffects(goreJson, goreId);

            var chaosGoreJson = GetJson("chaotic-gore-magala");
            var chaosGoreId = Database.AddMonsterAndGetId("chaotic-gore-magala");

            // Use clone for hitzones, proper json for everything else.
            ((JArray)goreJsonClone["monsterbodyparts"]).RemoveRange(0, 10);
            AddHitzones(goreJsonClone, "chaotic-gore-magala", chaosGoreId);

            AddStagger(chaosGoreJson, chaosGoreId);
            AddStatus(chaosGoreJson, chaosGoreId);
            AddItemEffects(chaosGoreJson, chaosGoreId);
            #endregion

            #region Dalamadur
            // Reason: Kiranico is missing the hitzone table for after parts are broken

            var dalaJson = GetJson("dalamadur");
            var dalaId = Database.AddMonsterAndGetId("dalamadur");

            var brokenDalaHitzones = new Dictionary<string, int[]>
            {
                { "Head (Broken)",       new[] { 55, 55, 30, 0, 0, 10, 10, 15 } },
                { "Front Legs (Broken)", new[] { 55, 55, 15, 5, 5, 10, 10, 20 } },
                { "Back Leg (Broken)",   new[] { 17, 17, 15, 5, 5, 10, 10, 15 } },
                { "Tail (Broken)",       new[] { 33, 33, 10, 5, 5, 10, 10, 15 } },
                { "Tail Tip (Broken)",   new[] { 75, 75, 60, 5, 5, 10, 10, 15 } }
            };

            AddHitzones(dalaJson, "dalamadur", dalaId);
            brokenDalaHitzones.ForEach(p => Database.AddHitzone(dalaId, Game.Four, p.Key, p.Value));

            AddStagger(dalaJson, dalaId);
            AddStatus(dalaJson, dalaId);
            AddItemEffects(dalaJson, dalaId);
            #endregion

            #region Shah Dalamadur
            // Reason: 3 hitzone tables; no stagger names

            var shahJson = GetJson("shah-dalamadur");
            var shahId = Database.AddMonsterAndGetId("shah-dalamadur");

            AddHitzonesWithThreeTables(shahJson, "shah-dalamadur", shahId);
            AddStatus(shahJson, shahId);
            AddItemEffects(shahJson, shahId);
            #endregion

            #region Gogmazios
            // Reason: 3 hitzone tables

            var gogJson = GetJson("gogmazios");
            var gogId = Database.AddMonsterAndGetId("gogmazios");

            AddHitzonesWithThreeTables(gogJson, "gogmazios", gogId);
            AddStagger(gogJson, gogId);
            AddStatus(gogJson, gogId);
            AddItemEffects(gogJson, gogId);
            #endregion

            #region Fatalis
            // Reason: No stagger names

            var fataJson = GetJson("fatalis");
            var fataId = Database.AddMonsterAndGetId("fatalis");

            AddHitzones(fataJson, "fatalis", fataId);
            AddStatus(fataJson, fataId);
            AddItemEffects(fataJson, fataId);
            #endregion

            #region Crimson Fatalis
            // Reason: No hitzones; no stagger names

            var crimJson = GetJson("crimson-fatalis");
            var crimId = Database.AddMonsterAndGetId("crimson-fatalis");

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
            crimHitzones.ForEach(p => Database.AddHitzone(crimId, Game.Four, p.Key, p.Value));

            AddStatus(crimJson, crimId);
            AddItemEffects(crimJson, crimId);
            #endregion

            #region White Fatalis
            // Reason: No hitzones; no stagger names

            var whiteJson = GetJson("white-fatalis");
            var whiteId = Database.AddMonsterAndGetId("white-fatalis");

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
            whiteHitzones.ForEach(p => Database.AddHitzone(whiteId, Game.Four, p.Key, p.Value));

            AddStatus(whiteJson, whiteId);
            AddItemEffects(whiteJson, whiteId);
            #endregion
        }
    }
}
