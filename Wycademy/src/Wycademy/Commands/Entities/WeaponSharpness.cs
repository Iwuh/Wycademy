using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wycademy.Commands.Enums;

namespace Wycademy.Commands.Entities
{
    public class WeaponSharpness
    {
        private Dictionary<SharpnessType, int> _values;

        [JsonConstructor]
        public WeaponSharpness(int red, int orange, int yellow, int green, int blue, int white, int purple)
        {
            _values = new Dictionary<SharpnessType, int>()
            {
                { SharpnessType.Red,    red     },
                { SharpnessType.Orange, orange  },
                { SharpnessType.Yellow, yellow  },
                { SharpnessType.Green,  green   },
                { SharpnessType.Blue,   blue    },
                { SharpnessType.White,  white   },
                { SharpnessType.Purple, purple  }
            };
        }

        public override string ToString() => _values.OrderByDescending(p => (int)p.Key).First(p => p.Value > 0).Key.ToString();
    }
}
