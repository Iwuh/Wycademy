using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class HornNotes
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
