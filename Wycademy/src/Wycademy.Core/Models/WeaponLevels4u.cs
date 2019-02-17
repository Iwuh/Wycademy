using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponLevels4u
    {
        public int Id { get; set; }
        public int CommonId { get; set; }
        public float Modifier { get; set; }
        public int FrenzyAffinity { get; set; }

        public virtual WeaponLevelsCommon Common { get; set; }
    }
}
