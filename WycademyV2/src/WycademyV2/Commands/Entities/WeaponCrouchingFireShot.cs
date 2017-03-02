using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponCrouchingFireShot
    {
        /// <summary>
        /// The shot name in all languages.
        /// </summary>
        public Dictionary<string, string> Names { get; private set; }

        /// <summary>
        /// How many shots can be fired in a single crouching fire.
        /// </summary>
        public int ClipSize { get; private set; }

        [JsonConstructor]
        public WeaponCrouchingFireShot(JObject pivot, JArray strings)
        {
            ClipSize = (int)pivot["capacity"];

            foreach (JObject item in strings)
            {
                Names.Add((string)item["loc"], (string)item["name"]);
            }
        }
    }
}
