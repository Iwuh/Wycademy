using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInternalShot
    {
        /// <summary>
        /// The shot name in each language.
        /// </summary>
        public Dictionary<string, string> Names { get; private set; }

        /// <summary>
        /// The total amount of the shot given.
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// The magazine size of the internal shot.
        /// </summary>
        public int Size { get; private set; }

        [JsonConstructor]
        public WeaponInternalShot(JObject pivot, JArray strings)
        {
            Total = (int)pivot["total"];
            Size = (int)pivot["count"];

            foreach (JObject item in strings)
            {
                Names.Add((string)item["loc"], (string)item["name"]);
            }
        }
    }
}
