using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class WeaponLevel
    {
        /// <summary>
        /// The weapon's display raw.
        /// </summary>
        [JsonProperty]
        public int Raw { get; set; }

        /// <summary>
        /// The weapon's true raw multiplier (null if it's a Gen weapon)
        /// </summary>
        [JsonProperty]
        public float? Modifier { get; set; }

        /// <summary>
        /// The weapon's affinity.
        /// </summary>
        [JsonProperty]
        public int Affinity { get; set; }

        /// <summary>
        /// The affinity bonus when overcoming the Frenzy Virus, or 0 if not applicable.
        /// </summary>
        [JsonProperty("affinity_virus")]
        public int FrenzyAffinity { get; set; }

        /// <summary>
        /// The bonus defense granted by having the weapon equipped.
        /// </summary>
        [JsonProperty]
        public int Defense { get; set; }

        /// <summary>
        /// The number of gem slots that the weapon has.
        /// </summary>
        [JsonProperty]
        public int Slots { get; set; }

        /// <summary>
        /// The weapon's various sharpnesses, or empty for gunner weapons.
        /// </summary>
        [JsonProperty("sharpness")]
        public List<WeaponSharpness> Sharpnesses { get; set; }

        /// <summary>
        /// The weapon's elemental/status effects.
        /// </summary>
        /// <remarks>0 if there are none, 1 if there is, 2 only for DB with 2 elements.</remarks>
        [JsonProperty("elements")]
        public List<WeaponEffect> Effects { get; set; }

        /// <summary>
        /// The Hunting Horn's notes, or null if the weapon is not a Hunting Horn.
        /// </summary>
        [JsonProperty("hhnotes")]
        public List<string> HornNotes { get; set; }

        /// <summary>
        /// The Gunlance's shell type & level, or null if the weapon is not a Gunlance.
        /// </summary>
        [JsonProperty("glshells")]
        public GunlanceShells GunlanceShells { get; set; }

        /// <summary>
        /// The Switch Axe/Charge Blade phial, or null if the weapon is not a SA/CB.
        /// </summary>
        [JsonProperty("phials")]
        public SaCbPhial Phial { get; set; }

        /// <summary>
        /// The Bow arc shot and charge shots, or null if the weapon is not a Bow.
        /// </summary>
        [JsonProperty]
        public BowShots BowShots { get; set; }

        /// <summary>
        /// The Bow's usable coatings, or null if the weapon is not a Bow.
        /// </summary>
        [JsonProperty]
        public List<string> BowCoatings { get; set; }

        /// <summary>
        /// The gun's stats and usable shots, or null if the weapon is not a LBG/HBG.
        /// </summary>
        [JsonProperty]
        public GunStats GunStats { get; set; }
    }
}
