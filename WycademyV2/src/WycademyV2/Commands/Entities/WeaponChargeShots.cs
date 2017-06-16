using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponChargeShots
    {
        /// <summary>
        /// The shot type.
        /// </summary>
        public BowChargeShot ShotType { get; private set; }

        /// <summary>
        /// Whether or not the charge shot requires Load Up to use.
        /// </summary>
        public bool LoadUp { get; private set; }

        [JsonConstructor]
        public WeaponChargeShots(JObject pivot)
        {
            ShotType = (BowChargeShot)((int)pivot["cshot_id"]);
            LoadUp = Convert.ToBoolean((int)pivot["loading"]);
        }
    }
}
