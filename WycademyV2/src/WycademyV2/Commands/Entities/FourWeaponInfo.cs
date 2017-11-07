using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WycademyV2.Commands.Entities
{
    public class FourWeaponInfo : BaseWeaponInfo
    {
        /// <summary>
        /// The weapon that this weapon upgrades into.
        /// </summary>
        [JsonProperty("upgrades_into_id")]
        public int? UpgradesIntoId { get; set; }
    }
}
