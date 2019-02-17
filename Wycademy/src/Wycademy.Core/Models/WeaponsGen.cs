using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class WeaponsGen
    {
        public int Id { get; set; }
        public int CommonId { get; set; }
        public string FinalName { get; set; }
        public string Description { get; set; }
        public string FinalDescription { get; set; }

        public virtual WeaponsCommon Common { get; set; }
    }
}
