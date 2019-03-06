using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class GunStats
    {
        public GunStats()
        {
            GunCrouchingFireShots = new HashSet<GunCrouchingFireShot>();
            GunInternalShots = new HashSet<GunInternalShot>();
            GunRapidFireShots = new HashSet<GunRapidFireShot>();
            GunShots = new HashSet<GunShot>();
        }

        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ReloadSpeed { get; set; }
        public string Recoil { get; set; }
        public string Deviation { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
        public virtual ICollection<GunCrouchingFireShot> GunCrouchingFireShots { get; set; }
        public virtual ICollection<GunInternalShot> GunInternalShots { get; set; }
        public virtual ICollection<GunRapidFireShot> GunRapidFireShots { get; set; }
        public virtual ICollection<GunShot> GunShots { get; set; }
    }
}
