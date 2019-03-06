using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public partial class HornNotes
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public HornNote? Note1 { get; set; }
        public HornNote? Note2 { get; set; }
        public HornNote? Note3 { get; set; }

        public virtual WeaponLevelsCommon WeaponLevel { get; set; }
    }
}
