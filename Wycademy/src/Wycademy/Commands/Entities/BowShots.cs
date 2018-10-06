using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class BowShots
    {
        /// <summary>
        /// The arc shot type.
        /// </summary>
        [JsonProperty]
        public string ArcShot { get; set; }

        /// <summary>
        /// All the charge levels of the bow.
        /// </summary>
        [JsonProperty]
        public List<string> ChargeShots { get; set; }
    }
}
