using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

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

        /// <summary>
        /// The Hunting Horn notes (empty if the weapon is not a Hunting Horn).
        /// </summary>
        [JsonProperty("hhnotes")]
        public List<WeaponNotes> HHNotes { get; set; }

        /// <summary>
        /// The Gunlance shot types and levels (empty if the weapon is not a Gunlance).
        /// </summary>
        [JsonProperty("shots")]
        public List<WeaponShells> GLShells { get; set; }

        /// <summary>
        /// The Switch Axe / Charge Blade phials (empty if the weapon is not a Switch Axe or Charge Blade).
        /// </summary>
        [JsonProperty("phials")]
        public List<WeaponPhials> SACBPhials { get; set; }

        /// <summary>
        /// The usable Bow coatings (empty if the weapon is not a bow)
        /// </summary>
        public List<int> BowCoatings { get; private set; }

        /// <summary>
        /// The arc shot type of the bow.
        /// </summary>
        public BowArcShot? ArcShot { get; private set; }

        /// <summary>
        /// The available charge shots.
        /// </summary>
        [JsonProperty("cshots")]
        public List<WeaponChargeShots> ChargeShots { get; set; }

        [JsonConstructor]
        public WeaponLevel(JObject coatings, JArray ashots)
        {
            var enabledCoatings = new List<int>();
            if (coatings != null)
            {
                for (int i = 0; i <= 10; i++)
                {
                    enabledCoatings.Append((int)coatings[$"bottle_enable_{i}"]);
                }
            }
            BowCoatings = enabledCoatings;

            if (ashots.Count > 0)
            {
                ArcShot = (BowArcShot)((int)ashots[0]["pivot"]["ashot_id"]);
            }
            else
            {
                ArcShot = null;
            }
        }
    }
}
