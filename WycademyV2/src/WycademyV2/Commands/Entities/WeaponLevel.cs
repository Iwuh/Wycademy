using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponLevel
    {
        /// <summary>
        /// The level of the weapon that these stats apply to.
        /// </summary>
        [JsonProperty("level")]
        public int Level { get; set; }

        /// <summary>
        /// The raw damage of the level.
        /// </summary>
        [JsonProperty("attack")]
        public int RawDamage { get; set; }

        /// <summary>
        /// The defense boost that the level grants.
        /// </summary>
        [JsonProperty("defense")]
        public int DefenseBoost { get; set; }

        /// <summary>
        /// The affinity of the current level.
        /// </summary>
        [JsonProperty("affinity")]
        public int Affinity { get; set; }

        /// <summary>
        /// How many decoration slots the level has.
        /// </summary>
        [JsonProperty("slots")]
        public int Slots { get; set; }

        /// <summary>
        /// How many zenny it costs to upgrade or make the level.
        /// </summary>
        [JsonProperty("price")]
        public int Price { get; set; }

        /// <summary>
        /// The different sharpness amounts for the current level.
        /// </summary>
        [JsonProperty("sharpness")]
        public List<WeaponSharpness> Sharpness { get; set; }

        /// <summary>
        /// The element values for the level.
        /// </summary>
        [JsonProperty("elements")]
        public List<WeaponElement> Elements { get; set; }
    }
}
