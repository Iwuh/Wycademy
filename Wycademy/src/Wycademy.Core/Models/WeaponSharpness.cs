using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponSharpness
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public int SharpnessOrdinal { get; set; }
        public int Red { get; set; }
        public int Orange { get; set; }
        public int Yellow { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int White { get; set; }
        public int Purple { get; set; }

        public virtual WeaponLevel WeaponLevel { get; set; }
    }
}
