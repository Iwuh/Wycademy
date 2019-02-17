using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class GunStats
    {
        public GunStats()
        {
            GunCrouchingFireShots = new HashSet<GunCrouchingFireShots>();
            GunInternalShots = new HashSet<GunInternalShots>();
            GunRapidFireShots = new HashSet<GunRapidFireShots>();
            GunShots = new HashSet<GunShots>();
        }

        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ReloadSpeed { get; set; }
        public string Recoil { get; set; }
        public string Deviation { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
        public virtual ICollection<GunCrouchingFireShots> GunCrouchingFireShots { get; set; }
        public virtual ICollection<GunInternalShots> GunInternalShots { get; set; }
        public virtual ICollection<GunRapidFireShots> GunRapidFireShots { get; set; }
        public virtual ICollection<GunShots> GunShots { get; set; }
    }
}
