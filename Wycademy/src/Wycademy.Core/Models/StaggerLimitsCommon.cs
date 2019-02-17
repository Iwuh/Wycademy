using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class StaggerLimitsCommon
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public int Stagger { get; set; }
        public string ExtractColour { get; set; }

        public virtual Monsters Monster { get; set; }
        public virtual StaggerLimitsGen StaggerLimitsGen { get; set; }
        public virtual StaggerLimitsWorld StaggerLimitsWorld { get; set; }
    }
}
