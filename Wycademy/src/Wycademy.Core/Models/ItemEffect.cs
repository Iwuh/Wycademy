using System;
using System.Collections.Generic;
using Wycademy.Core.Enums;

namespace Wycademy.Core.Models
{
    public class ItemEffect
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public Game? Game { get; set; }
        public string Name { get; set; }
        public int Normal { get; set; }
        public int Enraged { get; set; }
        public int Fatigued { get; set; }

        public Monsters Monster { get; set; }
    }
}
