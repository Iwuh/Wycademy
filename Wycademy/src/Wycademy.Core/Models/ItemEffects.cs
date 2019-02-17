using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class ItemEffects
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public int Normal { get; set; }
        public int Enraged { get; set; }
        public int Fatigued { get; set; }

        public virtual Monsters Monster { get; set; }
    }
}
