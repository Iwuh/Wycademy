using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public partial class Hitzones
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public int Cut { get; set; }
        public int Impact { get; set; }
        public int Shot { get; set; }
        public int Fire { get; set; }
        public int Water { get; set; }
        public int Ice { get; set; }
        public int Thunder { get; set; }
        public int Dragon { get; set; }

        public virtual Monsters Monster { get; set; }
    }
}
