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
        private List<WeaponInfo> _weapons;

        public WeaponInfoService()
        {
            _weapons = JsonConvert.DeserializeObject<List<WeaponInfo>>(File.ReadAllText(@"Data\weapon\greatsword.json"));
        }
    }
}
