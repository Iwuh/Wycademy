using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Entities
{
    public class WeaponElement
    {
        /// <summary>
        /// The element or status type.
        /// </summary>
        public WeaponEffect Effect { get; set; }

        /// <summary>
        /// The amount of element or status.
        /// </summary>
        public int Amount { get; set; }

        [JsonConstructor]
        public WeaponElement(JObject pivot)
        {
            Effect = (WeaponEffect)((int)pivot["element_id"]);
            Amount = (int)pivot["value"];
        }

        public override string ToString()
        {
            return $"{Amount} {Effect}";
        }
    }
}
