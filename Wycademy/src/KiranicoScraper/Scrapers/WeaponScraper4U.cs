using KiranicoScraper.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KiranicoScraper.Scrapers
{
    class WeaponScraper4U : Scraper
    {
        private const string BASE_URL = "http://kiranico.com/en/mh4u/weapon";

        private readonly Dictionary<int, WeaponType> TYPE_IDS = new Dictionary<int, WeaponType>
        {
            { 1,  WeaponType.GS     },
            { 2,  WeaponType.LS     },
            { 3,  WeaponType.SnS    },
            { 4,  WeaponType.DB     },
            { 5,  WeaponType.Hammer },
            { 6,  WeaponType.HH     },
            { 7,  WeaponType.Lance  },
            { 8,  WeaponType.GL     },
            { 9,  WeaponType.SA     },
            { 10, WeaponType.CB     },
            { 11, WeaponType.IG     },
            { 12, WeaponType.LBG    },
            { 13, WeaponType.HBG    },
            { 14, WeaponType.Bow    }
        };
        private readonly Dictionary<int, WeaponEffect> EFFECT_IDS = new Dictionary<int, WeaponEffect>
        {
            { 1, WeaponEffect.Fire    },
            { 2, WeaponEffect.Water   },
            { 3, WeaponEffect.Ice     },
            { 4, WeaponEffect.Thunder },
            { 5, WeaponEffect.Dragon  },
            { 6, WeaponEffect.Poison  },
            { 7, WeaponEffect.Para    },
            { 8, WeaponEffect.Sleep   },
            { 9, WeaponEffect.Blast   }
        };
        private readonly Dictionary<string, HornNote> HORN_NOTES = new Dictionary<string, HornNote>
        {
            { "white",  HornNote.White     },
            { "purple", HornNote.Purple    },
            { "red",    HornNote.Red       },
            { "blue",   HornNote.Blue      },
            { "yellow", HornNote.Yellow    },
            { "orange", HornNote.Orange    },
            { "green",  HornNote.Green     },
            { "sky",    HornNote.LightBlue }
        };

        /// <summary>
        /// Executes the scraper, adding 4U weapons to the database.
        /// </summary>
        public override void Execute()
        {
            foreach (var weaponClass in Database.ScraperLists.FourUltimate.Weapons)
            {
                // False tells it not to include the end substring in the returned slice.
                var weapons = Requester.GetJson($"{BASE_URL}/{weaponClass}", "{\"weapons\":", ";</script>", false);

                // Maps weapon ids in the json to their database ids, used for finding parents.
                var idMap = new Dictionary<int, int>();

                foreach (var weapon in weapons["weapons"])
                {
                    // Get the weapon's type, which is needed for type-specific stats.
                    var type = TYPE_IDS[(int)weapon["weapontype_id"]];

                    var name = (string)weapon["local_name"];
                    var rare = (string)weapon["rarity"];
                    var url = (string)weapon["link"];

                    // Get the weapon's parent id in the kiranico json.
                    var jsonParentId = ToInt(weapon["weapon_parent_id"]);
                    // If jsonParentId is 0, then the weapon has no parent. Otherwise, use the map to find the parent's database id using its json id.
                    var dbParentId = jsonParentId == 0 ? null as int? : idMap[jsonParentId];

                    // Add the weapon to the database and get its database id.
                    int dbId = Database.AddWeapon4U(name, type, rare, url, dbParentId);
                    // Get the weapon's id in the kiranico json.
                    var jsonId = ToInt(weapon["id"]);
                    // Add the two ids to the map so that we can look up the database id using the kiranico id.
                    idMap.Add(jsonId, dbId);

                    // ----------

                    var raw = ToInt(weapon["attack"]);
                    var affinity = ToInt(weapon["affinity"]);
                    var defense = ToInt(weapon["defense"]);

                    var slotCount = ToInt(weapon["slot"]);
                    var slots = new string('O', slotCount) + new string('-', 3 - slotCount);

                    var modifier = float.Parse((string)weapon["weapontype"]["modifier"]);
                    var frenzyAffinity = ToInt(weapon["affinity_virus"]);

                    // Add the level and get the id because we need it for the foreign key in type-specific tables.
                    int levelId = Database.AddWeaponLevel4U(dbId, raw, affinity, defense, slots, modifier, frenzyAffinity);

                    // ----------

                    // Weapons usually only have one effect but some dual blades have two.
                    var effects = (JArray)weapon["weaponspecialattacks"];
                    if (effects.Count != 0)
                    {
                        foreach (var effect in effects)
                        {
                            var pivot = effect["pivot"];

                            var effectType = EFFECT_IDS[ToInt(pivot["weaponspecialattack_id"])];
                            var effectAttack = ToInt(pivot["attack"]);
                            var needsAwaken = Convert.ToBoolean(ToInt(pivot["awaken_required"]));

                            Database.AddWeaponEffect(levelId, effectType, effectAttack, needsAwaken);
                        }
                    }

                    // All blademaster weapons have 2 sharpnesses: normal and with handicraft.
                    // NB: If there's no sharpnesses, the key will have a null value. This is _not_ the same as CLR null; it is still a valid JToken with a type of Null.
                    var sharpnesses = weapon["weaponsharpness"];
                    if (sharpnesses.Type != JTokenType.Null)
                    {
                        var baseSharpnesses = Database.ScraperLists.FourUltimate.SharpnessKeys.Select(k => ToInt(sharpnesses[k])).ToList();
                        // The Sharpness +1 values are represented by appending _plus to the end of each key.
                        var skillSharpnesses = Database.ScraperLists.FourUltimate.SharpnessKeys.Select(k => ToInt(sharpnesses[$"{k}_plus"])).ToList();

                        // Add the sharpnesses to the database.
                        Database.AddWeaponSharpness(levelId, 0, baseSharpnesses);
                        Database.AddWeaponSharpness(levelId, 1, skillSharpnesses);

                        // Generate an image for the sharpnesses.
                        SharpnessImageGenerator.GenerateImage(Game.Four, levelId, new[] { baseSharpnesses, skillSharpnesses });
                    }

                    // This is an array of 3 notes, or empty if the weapon is not a hunting horn.
                    var hornNotes = (JArray)weapon["weaponhuntinghornnotes"];
                    if (hornNotes.Count != 0)
                    {
                        var values = hornNotes.Select(o => HORN_NOTES[(string)o["color"]]).ToList();

                        Database.AddHornNotes(levelId, values);
                    }

                    // This is an array of one item, or empty if the weapon is not a gunlance.
                    var shells = ((JArray)weapon["weapongunlanceshells"]).FirstOrDefault();
                    if (shells != null)
                    {
                        var shellType = (string)shells["local_name"];
                        var shellLevel = ToInt(shells["pivot"]["level"]);

                        Database.AddGunlanceShells(levelId, shellType, shellLevel);
                    }

                    // This JArray is actually phials for both SA and CB. It's an array of one item, or empty if the weapon isn't an SA/CB.
                    var phials = ((JArray)weapon["weaponswitchaxephials"]).FirstOrDefault();
                    if (phials != null)
                    {
                        var phialType = (string)phials["local_name"];
                        Database.AddPhials(levelId, phialType, null);
                    }

                    // A JObject containing bow stats, or a null token if the weapon isn't a bow.
                    var bowStats = weapon["weaponbowconfigs"];
                    if (bowStats.Type != JTokenType.Null)
                    {
                        var arcShot = (string)bowStats["weaponbowarcshot"]["local_name"];

                        // Most bows have 3 charge levels, with a fourth requiring Load Up. Some only have 2, with a third requiring Load Up and no fourth level at all.
                        // In the latter case, weaponbowcharge_4 will be a null token so we simply return null for its charge type and then filter out any null values.
                        var chargeLevels = Utils.Sequence(1, 5, i =>
                        {
                            var token = bowStats[$"weaponbowcharge_{i}"];
                            if (token.Type == JTokenType.Null) return null;
                            return (string)token["local_name"];
                        })
                        .Where(s => s != null)
                        .ToArray();

                        // Get the name of each usable coating and remove the "Coating" suffix.
                        var coatings = weapon["weapongunnerammos"].Select(o =>
                        {
                            var coatingName = (string)o["local_name"];
                            var index = coatingName.IndexOf("Coating");
                            return coatingName.Substring(0, index);
                        }).ToArray();

                        Database.AddBowStats(levelId, arcShot, chargeLevels, coatings);
                    }

                    // A JObject containing LBG and HBG configs, null token if the weapon's not a gun.
                    var gunStats = weapon["weaponbowgunconfigs"];
                    if (gunStats.Type != JTokenType.Null)
                    {
                        // Fix inconsitent shot capacities.
                        if (name == "Buster Blaster+")
                        {
                            weapon["weaponbowgunconfigs"]["exhaust_1"] = "2";
                        }
                        else if (name == "Verdant Washbuckler")
                        {
                            weapon["weaponbowgunconfigs"]["exhaust_2"] = "0";
                        }

                        // Get reload speed, recoil, and deviation.
                        var gunStatValues = Database.ScraperLists.FourUltimate.GunStatKeys.Select(k => (string)gunStats[k]["local_name"]).ToList();

                        int gunStatsId = Database.AddGunStats(levelId, gunStatValues);

                        // For each pair in the dictionary, the key is the Kiranico json key and the value is the formatted English name.
                        var shots = Database.ScraperLists.FourUltimate.GunShots;
                        foreach (var shot in shots)
                        {
                            // Get the string representing the shot capacity. A capacity of "0" means that the gun can't load the shot and no skill can make the shot loadable.
                            var capacityString = (string)gunStats[shot.Key];
                            if (capacityString == "0") continue;

                            // If the capacity is enclosed in parentheses, it's not available by default and needs a skill to be loadable.
                            int capacity;
                            bool needsSkill;
                            var match = Regex.Match(capacityString, @"^\((\d{1,2})\)$");
                            if (match.Success)
                            {
                                capacity = int.Parse(match.Groups[1].Value);
                                needsSkill = true;
                            }
                            else
                            {
                                capacity = int.Parse(capacityString);
                                needsSkill = false;
                            }

                            Database.AddGunShot(gunStatsId, shot.Value, capacity, needsSkill);
                        }

                        /* Get an array of shots, needed to find rapid fire/crouch fire counts.
                         * 1. Create a collection of KeyValuePairs. The key is the shot's English name. To get the value, find the appropriate JObject in weapongunnerammos by
                         *      comparing it to the shot's English name. If no shot matches, use a null value, otherwise get the special capacity string.
                         * 2. Remove any elements with a null value or a capacity string of "-1" so that only rapid fire/crouching fire shots remain.
                         * 3. Convert the KeyValuePairs to a dictionary and parse the value strings.
                         */
                        var gunnerAmmos = weapon["weapongunnerammos"];
                        var specialShots = shots.Values
                            .Select(s => new KeyValuePair<string, JToken>(s, gunnerAmmos.FirstOrDefault(o => (string)o["local_name"] == s)?["pivot"]["capacity_special"]))
                            .Where(p => p.Value != null && (string)p.Value != "-1")
                            .ToDictionary(p => p.Key, p => ToInt(p.Value));

                        // Add each special shot as either a rapid fire or crouching fire shot.
                        foreach (var specialShot in specialShots)
                        {
                            if (type == WeaponType.LBG)
                            {
                                Database.AddGunRapidFireShot(gunStatsId, specialShot.Key, specialShot.Value);
                            }
                            else if (type == WeaponType.HBG)
                            {
                                Database.AddGunCrouchingFireShot(gunStatsId, specialShot.Key, specialShot.Value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts an integer type <see cref="JToken"/> to a CLR int.
        /// </summary>
        /// <param name="token">The <see cref="JToken"/> to parse.</param>
        /// <returns>The parsed <see cref="JToken"/>, or 0 if the token was of <see cref="JTokenType.Null"/>.</returns>
        private int ToInt(JToken token) => token.Type == JTokenType.Null ? 0 : int.Parse((string)token);
    }
}
