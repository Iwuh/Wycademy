using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class BowStats
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ArcShot { get; set; }
        public string[] ChargeShots { get; set; }
        public string[] Coatings { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
