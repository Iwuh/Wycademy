using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class GunShot
    {
        public int Id { get; set; }
        public int GunStatsId { get; set; }
        public string Name { get; set; }
        public int ClipSize { get; set; }
        public bool NeedsSkill { get; set; }

        public GunStats GunStats { get; set; }
    }
}
