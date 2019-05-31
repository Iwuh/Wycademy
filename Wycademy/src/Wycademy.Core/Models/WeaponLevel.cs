using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public abstract class WeaponLevel
    {
        protected WeaponLevel()
        {
            WeaponEffects = new List<WeaponEffect>();
            WeaponSharpnesses = new List<WeaponSharpness>();
        }

        public int Id { get; set; }
        public int WeaponId { get; set; }
        public Game? Game { get; set; }
        public int LevelOrdinal { get; set; }
        public int Raw { get; set; }
        public int Affinity { get; set; }
        public int Defense { get; set; }
        public string Slots { get; set; }

        public Weapon Weapon { get; set; }
        public BowStats BowStats { get; set; }
        public GunStats GunStats { get; set; }
        public GunlanceShellStats GunlanceShellStats { get; set; }
        public HornNotes HornNotes { get; set; }
        public Phial Phials { get; set; }
        public List<WeaponEffect> WeaponEffects { get; set; }
        public List<WeaponSharpness> WeaponSharpnesses { get; set; }
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
