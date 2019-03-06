using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class Phial
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string PhialType { get; set; }
        public int? PhialValue { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
