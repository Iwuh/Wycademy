using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class GunRapidFireShot
    {
        public int Id { get; set; }
        public int GunStatsId { get; set; }
        public string Name { get; set; }
        public int ShotCount { get; set; }
        public float? Modifier { get; set; }
        public string WaitTime { get; set; }

        public GunStats GunStats { get; set; }
    }
}
