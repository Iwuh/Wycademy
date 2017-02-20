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
        public WeaponNote Note1 { get; set; }

        /// <summary>
        /// The second note.
        /// </summary>
        public WeaponNote Note2 { get; set; }

        /// <summary>
        /// The third note.
        /// </summary>
        public WeaponNote Note3 { get; set; }

        [JsonConstructor]
        public WeaponNotes(int color_1, int color_2, int color_3)
        {
            Note1 = (WeaponNote)color_1;
            Note2 = (WeaponNote)color_2;
            Note3 = (WeaponNote)color_3;
        }
    }
}
