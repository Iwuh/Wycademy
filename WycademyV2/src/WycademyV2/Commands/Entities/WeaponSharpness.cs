using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Enums;

namespace WycademyV2.Commands.Entities
{
    public class WeaponSharpness
    {
        private Dictionary<SharpnessType, int> _values;

        [JsonConstructor]
        public WeaponSharpness(int red, int orange, int yellow, int green, int blue, int white)
        {
            _values = new Dictionary<SharpnessType, int>()
            {
                { SharpnessType.Red, red },
                { SharpnessType.Orange, orange },
                { SharpnessType.Yellow, yellow },
                { SharpnessType.Green, green },
                { SharpnessType.Blue, blue },
                { SharpnessType.White, white }
            };
        }

        public override string ToString()
        {
            var ordered = _values.OrderByDescending(p => (int)p.Key);
            var highest = ordered.FirstOrDefault(p => p.Value > 0);

            return highest.Key.ToString();
        }
    }
}
