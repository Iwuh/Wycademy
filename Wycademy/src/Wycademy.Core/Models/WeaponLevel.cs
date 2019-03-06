using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public abstract class WeaponLevel
    {
        protected WeaponLevel()
        {
            WeaponEffects = new HashSet<WeaponEffect>();
            WeaponSharpnesses = new HashSet<WeaponSharpness>();
        }

        public int Id { get; set; }
        public int WeaponId { get; set; }
        public Game? Game { get; set; }
        public int LevelOrdinal { get; set; }
        public int Raw { get; set; }
        public int Affinity { get; set; }
        public int Defense { get; set; }
        public string Slots { get; set; }

        public virtual WeaponsCommon Weapon { get; set; }
        public virtual BowStats BowStats { get; set; }
        public virtual GunStats GunStats { get; set; }
        public virtual GunlanceShellStats GunlanceShellStats { get; set; }
        public virtual HornNotes HornNotes { get; set; }
        public virtual Phial Phials { get; set; }
        public virtual ICollection<WeaponEffect> WeaponEffects { get; set; }
        public virtual ICollection<WeaponSharpness> WeaponSharpnesses { get; set; }
    }

    public class WeaponLevel4U : WeaponLevel
    {
        public float DisplayModifier { get; set; }
        public int FrenzyAffinity { get; set; }
    }

    public class WeaponLevelGen : WeaponLevel { }

    public class WeaponLevelWorld : WeaponLevel
    {
        public float DisplayModifier { get; set; }
    }
}
