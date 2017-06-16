using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Entities
{
    public class WeaponNotes
    {
        /// <summary>
        /// The first note (always white or purple).
        /// </summary>
        public HornNote Note1 { get; set; }

        /// <summary>
        /// The second note.
        /// </summary>
        public HornNote Note2 { get; set; }

        /// <summary>
        /// The third note.
        /// </summary>
        public HornNote Note3 { get; set; }

        [JsonConstructor]
        public WeaponNotes(int color_1, int color_2, int color_3)
        {
            Note1 = (HornNote)color_1;
            Note2 = (HornNote)color_2;
            Note3 = (HornNote)color_3;
        }

        public override string ToString()
        {
            return $"{Note1} / {Note2} / {Note3}";
        }
    }
}
