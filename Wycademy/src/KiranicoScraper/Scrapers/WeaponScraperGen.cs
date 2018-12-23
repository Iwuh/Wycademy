using KiranicoScraper.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KiranicoScraper.Scrapers
{
    class WeaponScraperGen : Scraper
    {
        private const string BASE_URL = "http://mhgen.kiranico.com";

        private readonly Dictionary<int, WeaponType> TYPE_IDS = new Dictionary<int, WeaponType>
        {
            { 0,  WeaponType.GS     },
            { 1,  WeaponType.SnS    },
            { 2,  WeaponType.Hammer },
            { 3,  WeaponType.Lance  },
            { 4,  WeaponType.HBG    },
            { 6,  WeaponType.LBG    },
            { 7,  WeaponType.LS     },
            { 8,  WeaponType.SA     },
            { 9,  WeaponType.GL     },
            { 10, WeaponType.Bow    },
            { 11, WeaponType.DB     },
            { 12, WeaponType.HH     },
            { 13, WeaponType.IG     },
            { 14, WeaponType.CB     }
        };

        private readonly Dictionary<int, WeaponEffect> EFFECT_IDS = new Dictionary<int, WeaponEffect>
        {
            { 1, WeaponEffect.Fire    },
            { 2, WeaponEffect.Water   },
            { 3, WeaponEffect.Thunder },
            { 4, WeaponEffect.Dragon  },
            { 5, WeaponEffect.Ice     },
            { 6, WeaponEffect.Poison  },
            { 7, WeaponEffect.Para    },
            { 8, WeaponEffect.Sleep   },
            { 9, WeaponEffect.Blast   }
        };

        private readonly Dictionary<int, HornNote> HORN_NOTES = new Dictionary<int, HornNote>()
        {
            { 1, HornNote.White     },
            { 2, HornNote.Purple    },
            { 3, HornNote.Red       },
            { 4, HornNote.Blue      },
            { 5, HornNote.Green     },
            { 6, HornNote.Yellow    },
            { 7, HornNote.LightBlue },
            { 8, HornNote.Orange    }
        };

        /// <summary>
        /// Executes the scraper, adding Gen weapon data to the database.
        /// </summary>
        public override void Execute()
        {
            foreach (var weaponClass in Database.ScraperLists.Generations.Weapons)
            {
                // With the default order, sometimes child weapons will be processed before their parents which causes issues. However, in testing I discovered that
                // all child weapons have an ID greater than their parent's ID, therefore ordering by ID ascending will prevent this issue.
                var weapons = Requester.GetJson($"{BASE_URL}/{weaponClass}", "[{\"id\":", "\"upgrades_from_level\":null}]").OrderBy(t => (int)t["id"]);

                // Map kiranico IDs to database IDs, for resolving parents.
                var idMap = new Dictionary<int, int>();

                foreach (var weapon in weapons)
                {
                    var type = TYPE_IDS[(int)weapon["weapontype_id"]];

                    var name = (string)weapon["strings"][0]["name"];
                    var description = (string)weapon["strings"][0]["description"];
                    var finalName = (string)weapon["strings"][0]["max_name"];
                    var finalDescription = (string)weapon["strings"][0]["max_description"];

                    var rareInt = (int)weapon["rare"];
                    // Rare 8 means Rare X, for deviant weapons.
                    var rare = rareInt == 8 ? "X" : rareInt.ToString();

                    var url = (string)weapon["url"];

                    // If the weapon's parent id in the kiranico json is null, it has no parent. Otherwise, get the parent's db id from the map.
                    var kiranicoParentId = (int?)weapon["upgrades_from"];
                    var dbParentId = kiranicoParentId.HasValue ? idMap[kiranicoParentId.Value] : null as int?;

                    var kiranicoId = (int)weapon["id"];
                    var weaponId = Database.AddWeaponGen(name, type, rare, url, dbParentId, finalName, description, finalDescription);
                    // Add the kiranico id and db id to the map for subsequent lookups.
                    idMap.Add(kiranicoId, weaponId);

                    // -------------------------------

                    foreach (var level in weapon["levels"])
                    {
                        // Used to ensure levels are displayed in the correct order.
                        var ordinal = (int)level["level"];

                        var raw = (int)level["attack"];
                        var affinity = (int)level["affinity"];
                        var defense = (int)level["defense"];

                        var slotCount = (int)level["slots"];
                        var slots = new string('O', slotCount) + new string('-', 3 - slotCount);

                        var levelId = Database.AddWeaponLevelGen(weaponId, ordinal, raw, affinity, defense, slots);

                        // -------------------------------

                        // Most weapons will only have one effect except for a handful of DBs.
                        var effects = (JArray)level["elements"];
                        if (effects.Count != 0)
                        {
                            foreach (var effect in effects)
                            {
                                var pivot = effect["pivot"];

                                var effectType = EFFECT_IDS[(int)pivot["element_id"]];
                                var effectValue = (int)pivot["value"];

                                Database.AddWeaponEffect(levelId, effectType, effectValue, false);
                            }
                        }

                        // All blademaster weapon levels will have 3 sharpness levels even if the weapon doesn't benefit from Handicraft.
                        var sharpnesses = (JArray)level["sharpness"];
                        if (sharpnesses.Count != 0)
                        {
                            foreach (var sharpness in sharpnesses)
                            {
                                // Get the values for red-white by their keys and then append 0 for purple.
                                var sharpnessValues = Database.ScraperLists.Generations.SharpnessKeys.Select(k => (int)sharpness[k]).Append(0).ToList();
                                Database.AddWeaponSharpness(levelId, sharpnesses.IndexOf(sharpness), sharpnessValues);
                            }
                        }

                        // hhnotes is an array of one object.
                        var hornNotes = ((JArray)level["hhnotes"]).FirstOrDefault();
                        if (hornNotes != null)
                        {
                            // Get each note's ID and convert it to a member of the HornNote enum.
                            var notes = Utils.Sequence(1, 4, i => HORN_NOTES[(int)hornNotes[$"color_{i}"]]).ToList();
                            Database.AddHornNotes(levelId, notes);
                        }

                        // Kiranico internally calls Gunlance shells "shots" and bowgun shots "shells". Why? It's also an array of one object.
                        var gunlanceShells = ((JArray)level["shots"]).FirstOrDefault();
                        if (gunlanceShells != null)
                        {
                            var shellType = Database.ScraperLists.Generations.GunlanceShellTypes[(int)gunlanceShells["pivot"]["shot_id"]];
                            var shellLevel = (int)gunlanceShells["pivot"]["level"];
                            Database.AddGunlanceShells(levelId, shellType, shellLevel);
                        }

                        // phials is an array of one object.
                        var phials = ((JArray)level["phials"]).FirstOrDefault();
                        if (phials != null)
                        {
                            var phialType = Database.ScraperLists.Generations.PhialTypes[(int)phials["pivot"]["phial_id"]];
                            var phialValue = (int)phials["pivot"]["value"];
                            // If phialValue is 0, the phial has no damage associated with it, so we pass null.
                            Database.AddPhials(levelId, phialType, phialValue == 0 ? null as int? : phialValue);
                        }

                        // ashots is an array of one item; if it's empty (i.e. FirstOrDefault returns null) then there is no arc shot.
                        var arcShot = ((JArray)level["ashots"]).FirstOrDefault();
                        // cshots is an array; if it's empty then there's no charge shots.
                        var chargeShots = (JArray)level["cshots"];
                        // coatings is an object; if it's null then there's no coating data. However, in this case, the json token's value is null. This is different from
                        // .NET null in that a null value is still a valid JToken, but one with a type of Null.
                        var coatings = level["coatings"];
                        if (arcShot != null && chargeShots.Count != 0 && coatings.Type != JTokenType.Null)
                        {
                            var arcShotType = Database.ScraperLists.Generations.ArcShotTypes[(int)arcShot["pivot"]["ashot_id"]];

                            var chargeShotNames = new List<string>();
                            for (int i = 0; i < 4; i++)
                            {
                                var pivot = chargeShots[i]["pivot"];
                                chargeShotNames.Add(Database.ScraperLists.Generations.ChargeShotTypes[(int)pivot["cshot_id"]]);

                                // All bows have a charge shot that requires Load Up. If the bow naturally has 3 levels, then the fourth level will require load up, so this
                                // if statement doesn't matter. However, if the bow only has 2 levels naturally, the third level will require load up, and there _is no_ fourth level.
                                // In this case, in the Kiranico json, the fourth level is a duplicate of the third, so we break after the first shot that requires load up.
                                if ((int)pivot["loading"] == 1)
                                    break;
                            }

                            // 1. Get each coating's english name and its level in the Kiranico json.
                            // 2. Remove any coatings with a level of 0.
                            // 3. If the coating has a level of 2 (special effect on this bow) surround the name with square brackets, otherwise just take the name.
                            var coatingNames = Database.ScraperLists.Generations.Coatings
                                .Select(p => (name: p.Value, level: (int)coatings[$"bottle_enable_{p.Key}"]))
                                .Where(t => t.level > 0)
                                .Select(t => t.level == 2 ? $"[{t.name}]" : t.name)
                                .ToArray();

                            Database.AddBowStats(levelId, arcShotType, chargeShotNames.ToArray(), coatingNames);
                        }

                        // MHGen Kiranico calls gun shots "shells" and gunlance shells "shots". This token will have a null value for non-gun weapons.
                        var gunShots = level["shells"];
                        // An array of the gun's internal shots, empty for non-gun weapons.
                        var internalShots = (JArray)level["uniqueshells"];
                        // Each is an array of one item, FirstOrDefault will return null if the array is empty.
                        var reloadSpeed = ((JArray)level["reloadspeeds"]).FirstOrDefault();
                        var recoil = ((JArray)level["recoils"]).FirstOrDefault();
                        var deviation = ((JArray)level["deviations"]).FirstOrDefault();
                        if (gunShots.Type != JTokenType.Null && 
                            internalShots.Count != 0 && 
                            reloadSpeed != null && recoil != null && deviation != null)
                        {
                            var gunStatsId = Database.AddGunStats(levelId, 
                                new[] 
                                {
                                    Database.ScraperLists.Generations.ReloadSpeeds[(int)reloadSpeed["pivot"]["reloadspeed_id"]],
                                    Database.ScraperLists.Generations.RecoilLevels[(int)recoil["pivot"]["recoil_id"]],
                                    Database.ScraperLists.Generations.DeviationLevels[(int)deviation["pivot"]["deviation_id"]]
                                });

                            foreach (var shot in Database.ScraperLists.Generations.GunShots)
                            {
                                // Get whether or not the shot is enabled for this gun. If it's a normal, pierce, pellet, crag, or clust shot (i.e. # <= 14), it can be enabled by a skill.
                                // Otherwise it's completely unavailable so we pass over it.
                                var enabled = (int)gunShots[$"shell_enable_{shot.Key}"];
                                if (enabled == 0 && shot.Key > 14) continue;

                                var capacity = (int)gunShots[$"shell_count_{shot.Key}"];

                                Database.AddGunShot(gunStatsId, shot.Value, capacity, enabled == 0);
                            }

                            foreach (var internalShot in internalShots)
                            {
                                var shotName = (string)internalShot["strings"][0]["name"];
                                var total = (int)internalShot["pivot"]["total"];
                                var clip = (int)internalShot["pivot"]["count"];

                                Database.AddGunInternalShot(gunStatsId, shotName, total, clip);
                            }

                            var crouchingFireShots = (JArray)level["squatshots"];
                            var rapidFireShots = (JArray)level["rapidshots"];
                            if (type == WeaponType.HBG && crouchingFireShots.Count != 0)
                            {
                                foreach (var shot in crouchingFireShots)
                                {
                                    var shotName = (string)shot["strings"][0]["name"];
                                    var count = (int)shot["pivot"]["capacity"];

                                    Database.AddGunCrouchingFireShot(gunStatsId, shotName, count);
                                }
                            }
                            else if (type == WeaponType.LBG && rapidFireShots.Count != 0)
                            {
                                foreach (var shot in rapidFireShots)
                                {
                                    var shotName = (string)shot["strings"][0]["name"];
                                    var count = (int)shot["pivot"]["capacity"];
                                    // The multiplier is given as a percentage in the Kiranico json, we convert it to a float.
                                    var damageMultiplier = ((int)shot["pivot"]["multiplier"]) / 100.0f;
                                    var waitTime = Database.ScraperLists.Generations.RapidFireWaitTimes[(int)shot["pivot"]["rapidshotwait_id"]];

                                    Database.AddGunRapidFireShot(gunStatsId, shotName, count, damageMultiplier, waitTime);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
