using System;
using System.Collections.Generic;

namespace Wycademy.Core.Models
{
    public class WeaponSharpness
    {
        public int Id { get; set; }
        public int WeaponLevelId { get; set; }
        public int SharpnessOrdinal { get; set; }
        public int Red { get; set; }
        public int Orange { get; set; }
        public int Yellow { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int White { get; set; }
        public int Purple { get; set; }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Red;
                    case 1:
                        return Orange;
                    case 2:
                        return Yellow;
                    case 3:
                        return Green;
                    case 4:
                        return Blue;
                    case 5:
                        return White;
                    case 6:
                        return Purple;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be be be in range [0, 7).");
                }
            }
        }

        public WeaponLevel WeaponLevel { get; set; }
    }
}
