using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class GunlanceShells
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public string ShellType { get; set; }
        public int ShellLevel { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
