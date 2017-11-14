using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponEffect
    {
        /// <summary>
        /// The type of the weapon effect, ex: Fire.
        /// </summary>
        [JsonProperty("id")]
        public WeaponEffectType Type { get; set; }

        /// <summary>
        /// How much damage this element/status has.
        /// </summary>
        [JsonProperty]
        public int Value { get; set; }

        /// <summary>
        /// Whether awaken is required to use the weapon's effect.
        /// </summary>
        [JsonProperty("need_awaken")]
        public bool NeedsAwaken { get; set; }
    }
}
