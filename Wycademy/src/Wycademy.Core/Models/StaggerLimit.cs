using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public abstract class StaggerLimit
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public Game? Game { get; set; }

        public string Name { get; set; }
        public int Stagger { get; set; }
        public string ExtractColour { get; set; }

        public Monsters Monster { get; set; }
    }

    public class StaggerLimit4U : StaggerLimit { }

    public class StaggerLimitGen : StaggerLimit
    {
        public int? Sever { get; set; }
    }

    public class StaggerLimitWorld : StaggerLimit
    {
        public int? Break { get; set; }
        public int? Sever { get; set; }
    }
}
