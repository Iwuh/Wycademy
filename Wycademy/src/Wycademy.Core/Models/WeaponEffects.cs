﻿using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponEffects
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public int Attack { get; set; }
        public bool NeedsAwaken { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
