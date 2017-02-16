using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponString
    {
        /// <summary>
        /// The in-progress name of the weapon.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The in-progress description of the weapon.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The fully-upgraded name of the weapon.
        /// </summary>
        [JsonProperty("max_name")]
        public string FinalName { get; set; }

        /// <summary>
        /// The fully-upgraded description of the weapon.
        /// </summary>
        [JsonProperty("max_description")]
        public string FinalDescription { get; set; }

        /// <summary>
        /// The language of the names and descriptions.
        /// </summary>
        [JsonProperty("loc")]
        public string Language { get; set; }
    }
}
