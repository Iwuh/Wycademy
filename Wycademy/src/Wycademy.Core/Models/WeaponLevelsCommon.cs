using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponLevelsCommon
    {
        public WeaponLevelsCommon()
        {
            WeaponEffects = new HashSet<WeaponEffects>();
            WeaponSharpnesses = new HashSet<WeaponSharpnesses>();
        }

        public int Id { get; set; }
        public int WeaponId { get; set; }
        public int LevelOrdinal { get; set; }
        public int Raw { get; set; }
        public int Affinity { get; set; }
        public int Defense { get; set; }
        public string Slots { get; set; }

        public virtual WeaponsCommon Weapon { get; set; }
        public virtual BowStats BowStats { get; set; }
        public virtual GunStats GunStats { get; set; }
        public virtual GunlanceShells GunlanceShells { get; set; }
        public virtual HornNotes HornNotes { get; set; }
        public virtual Phials Phials { get; set; }
        public virtual WeaponLevels4u WeaponLevels4u { get; set; }
        public virtual WeaponLevelsWorld WeaponLevelsWorld { get; set; }
        public virtual ICollection<WeaponEffects> WeaponEffects { get; set; }
        public virtual ICollection<WeaponSharpnesses> WeaponSharpnesses { get; set; }
    }
}
