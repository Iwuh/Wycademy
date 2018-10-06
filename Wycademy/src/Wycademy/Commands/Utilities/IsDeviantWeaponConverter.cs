using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wycademy.Commands.Utilities
{
    /// <summary>
    /// Checks if a weapon is a deviant weapon or not.
    /// </summary>
    public class IsDeviantWeaponConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var input = (long)reader.Value;

            return input == 1;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var output = (bool)value;

            writer.WriteValue(Convert.ToInt32(output));
        }
    }
}
