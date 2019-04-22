using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class GunStats
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ReloadSpeed { get; set; }
        public string Recoil { get; set; }
        public string Deviation { get; set; }

        public WeaponLevel WeaponLevel { get; set; }
        public List<GunCrouchingFireShot> GunCrouchingFireShots { get; set; }
        public List<GunInternalShot> GunInternalShots { get; set; }
        public List<GunRapidFireShot> GunRapidFireShots { get; set; }
        public List<GunShot> GunShots { get; set; }
    }
}
