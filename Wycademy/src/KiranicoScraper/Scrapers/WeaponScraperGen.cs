using KiranicoScraper.Scrapers.Lists;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Wycademy.Core.Enums;

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

        private readonly Dictionary<int, WeaponEffectType> EFFECT_IDS = new Dictionary<int, WeaponEffectType>
        {
            { 1, WeaponEffectType.Fire    },
            { 2, WeaponEffectType.Water   },
            { 3, WeaponEffectType.Thunder },
            { 4, WeaponEffectType.Dragon  },
            { 5, WeaponEffectType.Ice     },
            { 6, WeaponEffectType.Poison  },
            { 7, WeaponEffectType.Para    },
            { 8, WeaponEffectType.Sleep   },
            { 9, WeaponEffectType.Blast   }
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
            foreach (string weaponClass in ScraperListCollection.Generations.Weapons)
            {
                using (WebResponse response = Requester.GetPage($"{BASE_URL}/{weaponClass}"))
                {
                    // With the default order, sometimes child weapons will be processed before their parents which causes issues. However, in testing I discovered that
                    // all child weapons have an ID greater than their parent's ID, therefore ordering by ID ascending will prevent this issue.
                    var weapons = response.GetPageAsJson("[{\"id\":", "\"upgrades_from_level\":null}]").OrderBy(t => (int)t["id"]);

                    // Map kiranico IDs to database IDs, for resolving parents.
                    var idMap = new Dictionary<int, int>();

                    foreach (JToken weapon in weapons)
                    {
                        var builder = response.CreateWeaponBuilder();

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

                        builder.InitialiseWeaponGen(name, type, rare, url, dbParentId, finalName, description, finalDescription);

                        // -------------------------------

                        foreach (var level in weapon["levels"])
                        {
                            var levelBuilder = builder.CreateWeaponLevelBuilder();

                            // Used to ensure levels are displayed in the correct order.
                            var ordinal = (int)level["level"];

                            var raw = (int)level["attack"];
                            var affinity = (int)level["affinity"];
                            var defense = (int)level["defense"];

                            var slotCount = (int)level["slots"];
                            var slots = new string('O', slotCount) + new string('-', 3 - slotCount);

                            levelBuilder.InitialiseLevelGen(raw, affinity, defense, slots);

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

                                    // The awaken skill doesn't exist in Gen.
                                    levelBuilder.AddWeaponEffect(effectType, effectValue, false);
                                }
                            }

                            // All blademaster weapon levels will have 3 sharpness levels even if the weapon doesn't benefit from Handicraft.
                            var sharpnesses = (JArray)level["sharpness"];
                            if (sharpnesses.Count != 0)
                            {
                                foreach (var sharpness in sharpnesses)
                                {
                                    // Get the values for red-white by their keys.
                                    var sharpnessValues = ScraperListCollection.Generations.SharpnessKeys.Select(k => (int)sharpness[k]).ToList();

                                    // Then append 0 for purple and add the values to the database.
                                    sharpnessValues.Add(0);
                                    levelBuilder.AddWeaponSharpness(sharpnesses.IndexOf(sharpness), sharpnessValues);
                                }
                            }

                            // hhnotes is an array of one object.
                            var hornNotes = ((JArray)level["hhnotes"]).FirstOrDefault();
                            if (hornNotes != null)
                            {
                                // Get each note's ID and convert it to a member of the HornNote enum.
                                var notes = Utils.Sequence(1, 4, i => HORN_NOTES[(int)hornNotes[$"color_{i}"]]).ToList();
                                levelBuilder.AddHornNotes(notes);
                            }

                            // Kiranico internally calls Gunlance shells "shots" and bowgun shots "shells". Why? It's also an array of one object.
                            var gunlanceShells = ((JArray)level["shots"]).FirstOrDefault();
                            if (gunlanceShells != null)
                            {
                                var shellType = ScraperListCollection.Generations.GunlanceShellTypes[(int)gunlanceShells["pivot"]["shot_id"]];
                                var shellLevel = (int)gunlanceShells["pivot"]["level"];
                                levelBuilder.AddGunlanceShellStats(shellType, shellLevel);
                            }

                            // phials is an array of one object.
                            var phials = ((JArray)level["phials"]).FirstOrDefault();
                            if (phials != null)
                            {
                                var phialType = ScraperListCollection.Generations.PhialTypes[(int)phials["pivot"]["phial_id"]];
                                var phialValue = (int)phials["pivot"]["value"];
                                // If phialValue is 0, the phial has no damage associated with it, so we pass null.
                                levelBuilder.AddPhials(phialType, phialValue == 0 ? null as int? : phialValue);
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
                                var arcShotType = ScraperListCollection.Generations.ArcShotTypes[(int)arcShot["pivot"]["ashot_id"]];

                                var chargeShotNames = new List<string>();
                                for (int i = 0; i < 4; i++)
                                {
                                    var pivot = chargeShots[i]["pivot"];
                                    chargeShotNames.Add(ScraperListCollection.Generations.ChargeShotTypes[(int)pivot["cshot_id"]]);

                                    // All bows have a charge shot that requires Load Up. If the bow naturally has 3 levels, then the fourth level will require load up, so this
                                    // if statement doesn't matter. However, if the bow only has 2 levels naturally, the third level will require load up, and there _is no_ fourth level.
                                    // In this case, in the Kiranico json, the fourth level is a duplicate of the third, so we break after the first shot that requires load up.
                                    if ((int)pivot["loading"] == 1)
                                        break;
                                }

                                // 1. Get each coating's english name and its level in the Kiranico json.
                                // 2. Remove any coatings with a level of 0.
                                // 3. If the coating has a level of 2 (special effect on this bow) surround the name with square brackets, otherwise just take the name.
                                var coatingNames = ScraperListCollection.Generations.Coatings
                                    .Select(p => (name: p.Value, level: (int)coatings[$"bottle_enable_{p.Key}"]))
                                    .Where(t => t.level > 0)
                                    .Select(t => t.level == 2 ? $"[{t.name}]" : t.name)
                                    .ToArray();

                                levelBuilder.AddBowStats(arcShotType, chargeShotNames.ToArray(), coatingNames);
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
                                levelBuilder.AddGunStats(
                                    new[]
                                    {
                                        ScraperListCollection.Generations.ReloadSpeeds[(int)reloadSpeed["pivot"]["reloadspeed_id"]],
                                        ScraperListCollection.Generations.RecoilLevels[(int)recoil["pivot"]["recoil_id"]],
                                        ScraperListCollection.Generations.DeviationLevels[(int)deviation["pivot"]["deviation_id"]]
                                    });

                                foreach (var shot in ScraperListCollection.Generations.GunShots)
                                {
                                    // Get whether or not the shot is enabled for this gun. If it's a normal, pierce, pellet, crag, or clust shot (i.e. # <= 14), it can be enabled by a skill.
                                    // Otherwise it's completely unavailable so we pass over it.
                                    var enabled = (int)gunShots[$"shell_enable_{shot.Key}"];
                                    if (enabled == 0 && shot.Key > 14) continue;

                                    var capacity = (int)gunShots[$"shell_count_{shot.Key}"];

                                    levelBuilder.AddGunShot(shot.Value, capacity, enabled == 0);
                                }

                                foreach (var internalShot in internalShots)
                                {
                                    var shotName = (string)internalShot["strings"][0]["name"];
                                    var total = (int)internalShot["pivot"]["total"];
                                    var clip = (int)internalShot["pivot"]["count"];

                                    levelBuilder.AddGunInternalShot(shotName, total, clip);
                                }

                                var crouchingFireShots = (JArray)level["squatshots"];
                                var rapidFireShots = (JArray)level["rapidshots"];
                                if (type == WeaponType.HBG && crouchingFireShots.Count != 0)
                                {
                                    foreach (var shot in crouchingFireShots)
                                    {
                                        var shotName = (string)shot["strings"][0]["name"];
                                        var count = (int)shot["pivot"]["capacity"];

                                        levelBuilder.AddGunCrouchingFireShot(shotName, count);
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
                                        var waitTime = ScraperListCollection.Generations.RapidFireWaitTimes[(int)shot["pivot"]["rapidshotwait_id"]];

                                        levelBuilder.AddGunRapidFireShot(shotName, count, damageMultiplier, waitTime);
                                    }
                                }
                            }
                        }

                        // Add the kiranico id and the database id to the map for future parent lookups.
                        int dbId = builder.Commit();
                        idMap.Add((int)weapon["id"], dbId);
                    } 
                }
            }
        }
    }
}
