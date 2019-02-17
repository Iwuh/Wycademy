using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class StaggerLimitsGen
    {
        public int Id { get; set; }
        public int CommonId { get; set; }
        public int? Sever { get; set; }

        public virtual StaggerLimitsCommon Common { get; set; }
    }
}
