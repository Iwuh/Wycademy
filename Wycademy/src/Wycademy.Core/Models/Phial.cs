using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class Phial
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string PhialType { get; set; }
        public int? PhialValue { get; set; }

        public WeaponLevel WeaponLevel { get; set; }
    }
}
