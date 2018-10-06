using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class GunlanceShells
    {
        /// <summary>
        /// The gunlance shell type.
        /// </summary>
        [JsonProperty]
        public string Type { get; set; }

        /// <summary>
        /// The shell type's level.
        /// </summary>
        [JsonProperty]
        public string Level { get; set; }
    }
}
