using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;

namespace WycademyV2.Commands.Services
{
    public class WeaponInfoService
    {
        private readonly string[] FILENAMES = new string[] { "bow", "chargeblade", "dualblades", "greatsword", "gunlance", "hammer", "heavybowgun", "huntinghorn", "insectglaive", "lance", "lightbowgun", "longsword", "switchaxe", "swordshield" };

        private List<WeaponInfo> _weapons;

        public WeaponInfoService()
        {
            var deserialized = new List<List<WeaponInfo>>();
            foreach (var name in FILENAMES)
            {
                deserialized.Add(JsonConvert.DeserializeObject<List<WeaponInfo>>(File.ReadAllText($"{WycademyConst.DATA_LOCATION}\\weapon\\{name}.json")));
            }
            // Flatten the list of lists into a single list.
            _weapons = deserialized.SelectMany(x => x).ToList();
        }
    }
}
