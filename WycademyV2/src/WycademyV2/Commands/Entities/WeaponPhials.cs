using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponPhials
    {
        public SaCbPhial PhialType { get; set; }

        public int Value { get; set; }

        [JsonConstructor]
        public WeaponPhials(JObject pivot)
        {
            PhialType = (SaCbPhial)((int)pivot["phial_id"]);
            Value = (int)pivot["value"];
        }
    }
}
