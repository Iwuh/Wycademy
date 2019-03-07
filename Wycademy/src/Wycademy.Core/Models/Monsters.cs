using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class Monsters
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

        public ICollection<Hitzone> Hitzones { get; set; }
        public ICollection<ItemEffect> ItemEffects { get; set; }
        public ICollection<StaggerLimit> StaggerLimits { get; set; }
        public ICollection<StatusEffect> StatusEffects { get; set; }
    }
}
