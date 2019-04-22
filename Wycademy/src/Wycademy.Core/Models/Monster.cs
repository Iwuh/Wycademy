using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class Monster
    {
        public int Id { get; set; }
        public string WebName { get; set; }
        public string ProperName { get; set; }

        public List<Hitzone> Hitzones { get; set; }
        public List<ItemEffect> ItemEffects { get; set; }
        public List<StaggerLimit> StaggerLimits { get; set; }
        public List<StatusEffect> StatusEffects { get; set; }
    }
}
