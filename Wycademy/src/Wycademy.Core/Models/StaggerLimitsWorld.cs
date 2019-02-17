using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class StaggerLimitsWorld
    {
        public int Id { get; set; }
        public int CommonId { get; set; }
        public int? Break { get; set; }
        public int? Sever { get; set; }

        public virtual StaggerLimitsCommon Common { get; set; }
    }
}
