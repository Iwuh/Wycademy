using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfo
    {
        /// <summary>
        /// The unique identifier for the weapon (applies to all levels).
        /// </summary>
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// What weapon class the weapon belongs to.
        /// </summary>
        public WeaponType Weapon { get; set; }

        /// <summary>
        /// The maximum level the weapon can be upgraded to.
        /// </summary>
        [JsonProperty("max_lv")]
        public int MaxLevel { get; set; }

        /// <summary>
        /// The weapon's Rare level. 1-7 are the standard levels, 8 is RARE X.
        /// </summary>
        [JsonProperty("rare")]
        public int RareLevel { get; set; }

        /// <summary>
        /// Whether or not the weapon is made from Deviant parts.
        /// </summary>
        [JsonProperty("named")]
        [JsonConverter(typeof(IsDeviantWeaponConverter))]
        public bool IsDeviant { get; set; }

        /// <summary>
        /// The weapon's special ability.
        /// </summary>
        [JsonProperty("special_skill")]
        [JsonConverter(typeof(WeaponAbilityConverter))]
        public string SpecialAbility { get; set; }

        /// <summary>
        /// The name of the in progress and final version of the weapon.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The Kiranico URL for the weapon.
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }

        /// <summary>
        /// The weapon line that this weapon upgrades from.
        /// </summary>
        [JsonProperty("upgrades_from")]
        public int? UpgradesFrom { get; set; }

        /// <summary>
        /// The level this weapon upgrades from the previous one at.
        /// </summary>
        [JsonProperty("upgrades_from_level")]
        public int? UpgradesFromLevel { get; set; }

        /// <summary>
        /// The weapon names and descriptions for all languages.
        /// </summary>
        [JsonProperty("strings")]
        public List<WeaponString> Strings { get; set; }

        /// <summary>
        /// Each level of the weapon and it's stats.
        /// </summary>
        [JsonProperty("levels")]
        public List<WeaponLevel> Levels { get; set; }

        [JsonConstructor]
        public WeaponInfo(int weapontype_id)
        {
            Weapon = (WeaponType)weapontype_id;
        }
    }
}
