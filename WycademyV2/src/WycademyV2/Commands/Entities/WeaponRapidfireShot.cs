using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponRapidfireShot
    {
        /// <summary>
        /// The names of the shot in all languages.
        /// </summary>
        public Dictionary<string, string> Names { get; private set; }

        /// <summary>
        /// How many are fired in a single volley.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The damage multiplier for each bullet.
        /// </summary>
        public int Multiplier { get; private set; }

        /// <summary>
        /// The delay between volleys.
        /// </summary>
        public RapidfireWaitTime Wait { get; private set; }

        [JsonConstructor]
        public WeaponRapidfireShot(JObject pivot, JArray strings)
        {
            Names = new Dictionary<string, string>();
            foreach (JObject item in strings)
            {
                Names.Add((string)item["loc"], (string)item["name"]);
            }

            Number = (int)pivot["capacity"];
            Multiplier = (int)pivot["multiplier"];
            Wait = (RapidfireWaitTime)((int)pivot["rapidshotwait_id"]);
        }

        public override string ToString()
        {
            return $"{Names["en"]} - {Number} shots / {Multiplier}% multiplier / {Wait} wait time";
        }
    }
}
