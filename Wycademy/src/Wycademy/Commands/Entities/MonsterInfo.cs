using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class MonsterInfo
    {
        /// <summary>
        /// All hitzones for the monster and the values of each hitzone.
        /// </summary>
        public Dictionary<string, List<string>> Hitzones { get; set; }

        /// <summary>
        /// The different body parts, and how long each takes to stagger, sever, and the extract colour it gives.
        /// </summary>
        public Dictionary<string, List<string>> Stagger { get; set; }

        /// <summary>
        /// Each status and its effectiveness.
        /// </summary>
        public Dictionary<string, List<string>> Status { get; set; }

        /// <summary>
        /// Different immobilizing items and their effectiveness.
        /// </summary>
        public Dictionary<string, List<string>> Items { get; set; }
    }
}
