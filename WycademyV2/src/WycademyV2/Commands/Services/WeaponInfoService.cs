using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Services
{
    public class WeaponInfoService
    {
        private readonly Dictionary<WeaponType, string> WEAPON_EMOTES = new Dictionary<WeaponType, string>()
        {
            { WeaponType.GS,     "<:great_sword:376232911059288065>"      },
            { WeaponType.LS,     "<:long_sword:376233035441373184>"       },
            { WeaponType.SnS,    "<:sword_and_shield:376233070657011712>" },
            { WeaponType.DB,     "<:dual_blades:376232893489479690>"      },
            { WeaponType.Hammer, "<:hammer:376232942768357387>"           },
            { WeaponType.HH,     "<:hunting_horn:376232972157714444>"     },
            { WeaponType.Lance,  "<:lance:376233004697387009>"            },
            { WeaponType.GL,     "<:gunlance:376232926158782464>"         },
            { WeaponType.SA,     "<:switch_axe:376233052541681675>"       },
            { WeaponType.CB,     "<:charge_blade:376232878591442955>"     },
            { WeaponType.IG,     "<:insect_glaive:376232987777564672>"    },
            { WeaponType.LBG,    "<:light_bowgun:376233020069380097>"     },
            { WeaponType.HBG,    "<:heavy_bowgun:376232956773007363>"     },
            { WeaponType.Bow,    "<:bow:376232862237720577>"              }
        };

        private readonly Dictionary<WeaponEffectType, string> EFFECT_EMOTES = new Dictionary<WeaponEffectType, string>()
        {
            { WeaponEffectType.Fire,    "<:mhFire:374256883504119809>"    },
            { WeaponEffectType.Water,   "<:mhWater:374256883206193155>"   },
            { WeaponEffectType.Thunder, "<:mhThunder:318815090088607744>" },
            { WeaponEffectType.Dragon,  "<:mhDragon:318815089992007680>"  },
            { WeaponEffectType.Ice,     "<:mhIce:318815089882824705>"     },
            { WeaponEffectType.Poison,  "<:mhPoison:229901925481578496>"  },
            { WeaponEffectType.Para,    "<:mhPara:229901831927496716>"    },
            { WeaponEffectType.Sleep,   "<:mhSleep:374256883378421761>"   },
            { WeaponEffectType.Blast,   "<:mhBlast:374256883558776842>"   }
        };

        private readonly Dictionary<WeaponEffectType, uint> EFFECT_COLOURS = new Dictionary<WeaponEffectType, uint>()
        {
            { WeaponEffectType.Fire,    0xFF4802 },
            { WeaponEffectType.Water,   0x93EBFF },
            { WeaponEffectType.Thunder, 0xFFFE03 },
            { WeaponEffectType.Dragon,  0x6B72B6 },
            { WeaponEffectType.Ice,     0xADCEF7 },
            { WeaponEffectType.Poison,  0x852886 },
            { WeaponEffectType.Para,    0xFFFF03 },
            { WeaponEffectType.Sleep,   0x72BA46 },
            { WeaponEffectType.Blast,   0x424343 }
        };

        private Dictionary<int, FourWeaponInfo> _fourWeapons;
        private Dictionary<int, GenWeaponInfo> _genWeapons;

        public WeaponInfoService()
        {
            _fourWeapons = LoadWeaponInfo<FourWeaponInfo>("4u");
            _genWeapons = LoadWeaponInfo<GenWeaponInfo>("gen");
        }

        public List<Embed> Build(BaseWeaponInfo weapon, IUser user)
        {
            if (weapon is FourWeaponInfo four)
            {
                var eb = new EmbedBuilder()
                    .WithAuthor("Wycademy", "https://cdn.discordapp.com/avatars/207172354101608448/67bb079bde2e9ed142ad824e4a31d5af.png", "https://github.com/Iwuh/Wycademy")
                    .WithTitle($"{weapon.Name} - {weapon.WeaponTypeName} (RARE {weapon.Rare})")
                    .WithUrl(weapon.Url)
                    .WithDescription($"Upgrades from: {GetNameById(_fourWeapons, weapon.UpgradesFromId)}\nUpgrades into: {GetNameById(_fourWeapons, four.UpgradesIntoId)}")
                    .WithFooter(user.ToString(), user.GetAvatarUrl())
                    .WithCurrentTimestamp();

                return new List<Embed>() { BuildLevel(eb, four.Levels.First(), four.WeaponType, false) };
            }
            else if (weapon is GenWeaponInfo gen)
            {
                var embeds = new List<Embed>();

                foreach (WeaponLevel level in gen.Levels)
                {
                    var description = new StringBuilder()
                        .AppendLine($"Upgrades from: {GetNameById(_fourWeapons, weapon.UpgradesFromId)}")
                        .AppendLine($"Name: {weapon.Name}")
                        .AppendLine($"Final Name: {gen.FinalName}")
                        .AppendLine($"Description: {gen.Description}")
                        .AppendLine($"Final Description: {gen.FinalDescription}");

                    var eb = new EmbedBuilder()
                        .WithAuthor("Wycademy", "https://cdn.discordapp.com/avatars/207172354101608448/67bb079bde2e9ed142ad824e4a31d5af.png", "https://github.com/Iwuh/Wycademy")
                        .WithTitle($"{weapon.Name} / {gen.FinalName} - {weapon.WeaponTypeName} (RARE {(weapon.Rare == "8" ? "X" : weapon.Rare)})")
                        .WithUrl(weapon.Url)
                        .WithDescription(description.ToString())
                        .WithFooter(user.ToString(), user.GetAvatarUrl())
                        .WithCurrentTimestamp();

                    embeds.Add(BuildLevel(eb, level, gen.WeaponType, true));
                }

                return embeds;
            }
            else
            {
                throw new ArgumentException("Argument is not a recognized implementation of BaseWeaponInfo.", nameof(weapon));
            }
        }

        /// <summary>
        /// Search for MH4U weapons matching a given search term.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        /// <returns>All MH4U weapons that match the given search term.</returns>
        public IEnumerable<FourWeaponInfo> SearchFour(string searchTerm)
        {
            var pairs = _fourWeapons.Where(p => p.Value.Name.ToLower().Contains(searchTerm.ToLower()));
            return pairs.Select(p => p.Value).ToList();
        }

        /// <summary>
        /// Search for MHGen weapons matching a given search term.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        /// <returns>All MHGen weapons that match the given search term.</returns>
        public IEnumerable<GenWeaponInfo> SearchGen(string searchTerm)
        {
            var pairs = _genWeapons.Where(p => p.Value.Name.ToLower().Contains(searchTerm.ToLower()) 
                                            || p.Value.FinalName.ToLower().Contains(searchTerm.ToLower()));
            return pairs.Select(p => p.Value).ToList();
        }
        
        /// <summary>
        /// Deserializes the weapon info for a game.
        /// </summary>
        /// <typeparam name="T">The implementation of <see cref="BaseWeaponInfo"/> to deserialize to.</typeparam>
        /// <param name="gameString">The directory name of the game.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the weapon's ID as the key and the weapon info as the value.</returns>
        private Dictionary<int, T> LoadWeaponInfo<T>(string gameString) 
            where T : BaseWeaponInfo
        {
            // The encoding to use when reading from files.
            var encoding = new UTF8Encoding(false);

            // Start by storing each weapon type as its own element in a list.
            var deserialized = new List<List<T>>();
            foreach (string fileName in Directory.GetFiles(Path.Combine(WycademyConst.DATA_LOCATION, gameString, "weapon")))
            {
                string json = File.ReadAllText(fileName, encoding);
                deserialized.Add(JsonConvert.DeserializeObject<List<T>>(json));
            }
            // Flatten the list of lists then convert it to a dictionary with the ID as the key.
            return deserialized.SelectMany(x => x).ToDictionary(w => w.Id);
        }

        private Embed BuildLevel(EmbedBuilder builder, WeaponLevel level, WeaponType type, bool isGen)
        {
            var stats = new StringBuilder();

            // Add the display raw, and true raw if it's a 4U weapon.
            if (isGen)
            {
                stats.AppendLine($"{level.Raw} raw");
            }
            else
            {
                stats.AppendLine($"{level.Raw} raw ({(int)(level.Raw / level.Modifier)} true)");
            }

            // Add the affinity and frenzy affinity, if applicable.
            if (isGen)
            {
                stats.AppendLine($"{level.Affinity}% affinity");
            }
            else
            {
                stats.AppendLine($"{level.Affinity}{(level.FrenzyAffinity != 0 ? "/" + level.FrenzyAffinity : string.Empty)}% affinity");
            }

            // Add the defense bonus.
            stats.AppendLine($"+{level.Defense} defense");

            // Add elements and set the embed colour.
            if (level.Effects.Count > 0)
            {
                builder.WithColor(EFFECT_COLOURS[level.Effects[0].Type]);

                // Certain Dual Blades can have two elements.
                foreach (WeaponEffect effect in level.Effects)
                {
                    string line = $"{effect.Value} {EFFECT_EMOTES[effect.Type]}";
                    if (effect.NeedsAwaken)
                    {
                        // Put parentheses around the effect if it requires awaken.
                        stats.AppendLine($"({line})");
                    }
                    else
                    {
                        stats.AppendLine(line);
                    }
                }
            }
            else
            {
                stats.AppendLine("No Element/Status");
            }

            // Add the weapon's slots.
            stats.AppendLine(new string('O', level.Slots) + new string('-', 3 - level.Slots));

            builder.AddField("Stats", stats.ToString());

            // As long as the weapon has at least one sharpness...
            if (level.Sharpnesses.Count > 0)
            {
                var sharpnessBuilder = new StringBuilder();
                // Add the base sharpness and the sharpness with S+1 if applicable.
                for (int i = 0; i < level.Sharpnesses.Count; i++)
                {
                    sharpnessBuilder.AppendLine($"{level.Sharpnesses[i]} (+{i})");
                }
                builder.AddField("Sharpnesses", sharpnessBuilder.ToString());
            }

            switch (type)
            {
                // Add the HH's notes.
                case WeaponType.HH:
                    builder.AddField($"{WEAPON_EMOTES[WeaponType.HH]} Notes", string.Join(" ", level.HornNotes));
                    break;

                // Add the GL's shell type and level.
                case WeaponType.GL:
                    builder.AddField($"{WEAPON_EMOTES[WeaponType.GL]} Shells", $"{level.GunlanceShells.Type} lv {level.GunlanceShells.Level}");
                    break;

                // Add the SA's phial type and value, if applicable.
                case WeaponType.SA:
                    if (level.Phial.Value != "0")
                    {
                        builder.AddField($"{WEAPON_EMOTES[WeaponType.SA]} Phial", $"{level.Phial.Value} {level.Phial.Type}");
                    }
                    else
                    {
                        builder.AddField($"{WEAPON_EMOTES[WeaponType.SA]} Phial", level.Phial.Type);
                    }
                    break;

                // Add the CB's phial type.
                case WeaponType.CB:
                    builder.AddField($"{WEAPON_EMOTES[WeaponType.CB]} Phials", level.Phial.Type);
                    break;

                // Add the general gun data and the LBG's rapid fire shots.
                case WeaponType.LBG:
                    AddUniversalGunFields(WeaponType.LBG, level.GunStats, builder);
                    string rapidFireShots;
                    if (isGen)
                    {
                        rapidFireShots = string.Join("\n", level.GunStats.RapidFireShots.Select(s => $"{s.Name} - {s.Count} shots / {s.Multiplier} multiplier / {s.WaitTime} wait time"));
                    }
                    else
                    {
                        rapidFireShots = string.Join("\n", level.GunStats.RapidFireShots.Select(s => $"{s.Name} ({s.Count})"));
                    }
                    builder.AddInlineField($"{WEAPON_EMOTES[WeaponType.LBG]} Rapid Fire Shots", rapidFireShots);
                    break;

                // Add the general gun data and the HBG's crouching fire shots.
                case WeaponType.HBG:
                    AddUniversalGunFields(WeaponType.HBG, level.GunStats, builder);
                    builder.AddInlineField($"{WEAPON_EMOTES[WeaponType.HBG]} Crouching Fire Shots", string.Join("\n", level.GunStats.CrouchingFireShots.Select(s => $"{s.Name} ({s.Count})")));
                    break;

                // Add the bow's arc shot, charge shots, and usable coatings.
                case WeaponType.Bow:
                    builder.AddInlineField($"{WEAPON_EMOTES[WeaponType.Bow]} Shots", $"Arc Shot: {level.BowShots.ArcShot}\n{string.Join("\n", level.BowShots.ChargeShots)}");
                    builder.AddInlineField($"{WEAPON_EMOTES[WeaponType.Bow]} Coatings", string.Join("\n", level.BowCoatings));
                    break;
            }

            return builder.Build();
        }

        private void AddUniversalGunFields(WeaponType type, GunStats stats, EmbedBuilder builder)
        {
            builder.AddInlineField($"{WEAPON_EMOTES[type]} Stats", $"Reload Speed: {stats.ReloadSpeed}\nRecoil: {stats.Recoil}\nDeviation: {stats.Deviation}");
            builder.AddInlineField($"{WEAPON_EMOTES[type]} Shots", string.Join("\n", stats.UsableShots.Select(s => $"{s.Name} ({s.Capacity})")));

            if (stats.InternalShots != null)
            {
                builder.AddInlineField($"{WEAPON_EMOTES[type]} Internal Shots", string.Join("\n", stats.InternalShots.Select(s => $"{s.Name} ({s.ClipSize} / {s.Total})")));
            }
        }

        private string GetNameById<T>(Dictionary<int, T> weapons, int? id) where T : BaseWeaponInfo
        {
            if (id.HasValue)
            {
                return weapons[id.Value].Name;
            }
            return "None";
        }
    }
}
