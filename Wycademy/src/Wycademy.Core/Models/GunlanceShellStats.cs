using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class GunlanceShellStats
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ShellType { get; set; }
        public int ShellLevel { get; set; }

        public WeaponLevel WeaponLevel { get; set; }
    }
}
