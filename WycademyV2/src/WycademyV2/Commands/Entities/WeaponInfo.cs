using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfo
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("weapontype_id")]
        public int WeaponTypeID { get; set; }

        [JsonProperty("max_lv")]
        public int MaxLevel { get; set; }

        [JsonProperty("rare")]
        public int RareLevel { get; set; }

        [JsonProperty("named")]
        [JsonConverter(typeof(IsDeviantWeaponConverter))]
        public bool IsDeviant { get; set; }

        [JsonProperty("special_skill")]
        [JsonConverter(typeof(WeaponAbilityConverter))]
        public string SpecialAbility { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("upgrades_from")]
        public int UpgradesFrom { get; set; }

        [JsonProperty("upgrades_from_level")]
        public int UpgradesFromLevel { get; set; }
    }
}
