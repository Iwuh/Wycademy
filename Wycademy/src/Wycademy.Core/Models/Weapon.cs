using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public abstract class Weapon
    {
        public int Id { get; set; }
        public Game? Game { get; set; }
        public string Name { get; set; }
        public WeaponType? WeaponType { get; set; }
        public string Rare { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }

        public virtual Weapon Parent { get; set; }
        public virtual List<Weapon> Children { get; set; }
        public virtual List<WeaponLevel> WeaponLevels { get; set; }
    }

    public class Weapon4U : Weapon { }

    public class WeaponGen : Weapon
    {
        public string FinalName { get; set; }
        public string Description { get; set; }
        public string FinalDescription { get; set; }
    }

    public class WeaponWorld : Weapon { }
}
