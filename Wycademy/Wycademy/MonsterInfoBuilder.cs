using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    static class MonsterInfoBuilder
    {
        // Used for statistics
        public static int Queries { get; private set; }

        // Json deserialization is cpu-bound work, so commands must use Task.Run to get a MonsterInfo without blocking.
        public static MonsterInfo GetMonsterInfo(string monster, string category)
        {
            // Read the content of a json file into a JObject
            JObject parsed;
            using (StreamReader sr = new StreamReader($"{WycademySettings.JSON_PATH}\\{monster}.json"))
            {
                parsed = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
            }

            MonsterInfo info = new MonsterInfo(category);
            info.Data = ConvertJsonFragmentToDictionary(parsed, category.ToLower());

            Queries++;
            return info;
        }

        private static Dictionary<string, string[]> ConvertJsonFragmentToDictionary(JObject full, string subsection)
        {
            // Gets a subsection of the full json, then casts it to an IDictionary<string, JToken>.
            // We then call ToDictionary on it and use LINQ magic to convert it to a Dictionary<string, string[]>
            return ((IDictionary<string, JToken>)full[subsection]).ToDictionary(pair => pair.Key, pair => ((string)pair.Value).Split(' '));
        }
    }
}
