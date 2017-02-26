using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponShells
    {
        public GunlanceShellType ShotType { get; set; }

        public int Level { get; set; }

        [JsonConstructor]
        public WeaponShells(JObject pivot)
        {
            ShotType = (GunlanceShellType)((int)pivot["shot_id"]);
            Level = (int)pivot["level"];
        }
    }
}
