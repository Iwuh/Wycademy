using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Utilities
{
    public class WeaponAbilityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var input = (int)reader.Value;

            switch (input)
            {
                case 1:
                    return "Roll to sharpen";
                case 2:
                    return "Roll to reload";
                default:
                    return "None";
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var output = (string)value;

            int valueToWrite;
            switch (output)
            {
                case "Roll to sharpen":
                    valueToWrite = 1;
                    break;
                case "Roll to reload":
                    valueToWrite = 2;
                    break;
                default:
                    valueToWrite = 0;
                    break;
            }

            writer.WriteValue(valueToWrite);
        }
    }
}
