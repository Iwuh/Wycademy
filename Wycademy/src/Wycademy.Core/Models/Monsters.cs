using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class Monsters
    {
        public Monsters()
        {
            Hitzones = new HashSet<Hitzone>();
            ItemEffects = new HashSet<ItemEffect>();
            StaggerLimits = new HashSet<StaggerLimit>();
            StatusEffects = new HashSet<StatusEffect>();
        }

        public int Id { get; set; }
        public string WebName { get; set; }
        public string ProperName { get; set; }

        public virtual ICollection<Hitzone> Hitzones { get; set; }
        public virtual ICollection<ItemEffect> ItemEffects { get; set; }
        public virtual ICollection<StaggerLimit> StaggerLimits { get; set; }
        public virtual ICollection<StatusEffect> StatusEffects { get; set; }
    }
}
