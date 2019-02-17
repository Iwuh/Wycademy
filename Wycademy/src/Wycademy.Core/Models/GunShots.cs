using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class GunShots
    {
        public int Id { get; set; }
        public int GunStatsId { get; set; }
        public string Name { get; set; }
        public int ClipSize { get; set; }
        public bool NeedsSkill { get; set; }

        public virtual GunStats GunStats { get; set; }
    }
}
