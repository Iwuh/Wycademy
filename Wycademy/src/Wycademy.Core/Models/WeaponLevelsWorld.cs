using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponLevelsWorld
    {
        public int Id { get; set; }
        public int CommonId { get; set; }
        public float Modifier { get; set; }

        public virtual WeaponLevelsCommon Common { get; set; }
    }
}
