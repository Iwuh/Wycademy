using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class StatusEffects
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public int InitialThreshold { get; set; }
        public int Increase { get; set; }
        public int MaxThreshold { get; set; }
        public int Duration { get; set; }
        public int Damage { get; set; }
        public int ReductionTime { get; set; }
        public int ReductionAmount { get; set; }

        public virtual Monsters Monster { get; set; }
    }
}
