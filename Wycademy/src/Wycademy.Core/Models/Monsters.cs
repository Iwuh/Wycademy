using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class Monsters
    {
        public Monsters()
        {
            Hitzones = new HashSet<Hitzones>();
            ItemEffects = new HashSet<ItemEffects>();
            StaggerLimitsCommon = new HashSet<StaggerLimitsCommon>();
            StatusEffects = new HashSet<StatusEffects>();
        }

        public int Id { get; set; }
        public string WebName { get; set; }
        public string ProperName { get; set; }

        public virtual ICollection<Hitzones> Hitzones { get; set; }
        public virtual ICollection<ItemEffects> ItemEffects { get; set; }
        public virtual ICollection<StaggerLimitsCommon> StaggerLimitsCommon { get; set; }
        public virtual ICollection<StatusEffects> StatusEffects { get; set; }
    }
}
