using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfo
    {
        [JsonProperty("id")]
        public int ID { get; set; }
    }
}
