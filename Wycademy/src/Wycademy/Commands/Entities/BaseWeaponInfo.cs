using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Wycademy.Commands.Enums;

namespace Wycademy.Commands.Entities
{
    /// <summary>
    /// Represents an abstract base class containing weapon info elements common to all MH games.
    /// </summary>
    public abstract class BaseWeaponInfo
    {
        /// <summary>
        /// The weapon's unique ID.
        /// </summary>
        [JsonProperty]
        public int Id { get; set; }

        /// <summary>
        /// The weapon's name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The weapon's type.
        /// </summary>
        [JsonProperty("weapon_type_id")]
        public WeaponType WeaponType { get; set; }

        /// <summary>
        /// The English name of the weapon's type.
        /// </summary>
        [JsonProperty("weapon_type_name")]
        public string WeaponTypeName { get; set; }

        /// <summary>
        /// The weapon's rarity level.
        /// </summary>
        [JsonProperty]
        public string Rare { get; set; }

        /// <summary>
        /// The ID of the weapon that this weapon upgrades from, or null if not applicable.
        /// </summary>
        [JsonProperty("upgrades_from_id")]
        public int? UpgradesFromId { get; set; }

        /// <summary>
        /// The URL of the weapon's page on Kiranico.
        /// </summary>
        [JsonProperty]
        public string Url { get; set; }

        /// <summary>
        /// All of the weapon's levels. For 4U weapons, this collection contains only one level.
        /// </summary>
        [JsonProperty]
        public List<WeaponLevel> Levels { get; set; }
    }
}
