using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponsCommon
    {
        public WeaponsCommon()
        {
            InverseParent = new HashSet<WeaponsCommon>();
            WeaponLevelsCommon = new HashSet<WeaponLevelsCommon>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Rare { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }

        public virtual WeaponsCommon Parent { get; set; }
        public virtual WeaponsGen WeaponsGen { get; set; }
        public virtual ICollection<WeaponsCommon> InverseParent { get; set; }
        public virtual ICollection<WeaponLevelsCommon> WeaponLevelsCommon { get; set; }
    }
}
