using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public partial class WeaponEffect
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public WeaponEffectType? EffectType { get; set; }
        public int Attack { get; set; }
        public bool NeedsAwaken { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
