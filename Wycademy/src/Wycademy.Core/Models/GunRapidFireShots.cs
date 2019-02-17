using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class GunRapidFireShots
    {
        public int Id { get; set; }
        public int GunStatsId { get; set; }
        public string Name { get; set; }
        public int ShotCount { get; set; }
        public float? Modifier { get; set; }
        public string WaitTime { get; set; }

        public virtual GunStats GunStats { get; set; }
    }
}
