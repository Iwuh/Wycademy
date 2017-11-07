using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WycademyV2.Commands.Entities
{
    public class GenWeaponInfo : BaseWeaponInfo
    {
        /// <summary>
        /// The weapon's name when upgraded to its maximum level.
        /// </summary>
        [JsonProperty("final_name")]
        public string FinalName { get; set; }

        /// <summary>
        /// The weapon's description.
        /// </summary>
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// The weapon's description when upgraded to its maximum level.
        /// </summary>
        [JsonProperty("final_description")]
        public string FinalDescription { get; set; }

        /// <summary>
        /// The maximum level of the weapon.
        /// </summary>
        [JsonProperty("max_level")]
        public int MaxLevel { get; set; }
    }
}
